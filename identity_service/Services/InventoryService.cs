using IdentityService.Data;
using IdentityService.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Services
{
    public interface IInventoryService
    {
        Task<SeatInventoryDto?> GetInventoryAsync(string flightId);
        Task<List<SeatInventoryDto>> GetAllInventoryAsync();
        Task<BookingResponseDto> BookSeatsAsync(string flightId, string passengerName, int seats);
        Task<bool> CancelSeatsAsync(string flightId, int seats);
        Task<double?> GetOccupancyRateAsync(string flightId);
        Task InitializeInventoryAsync(List<Flight> flights);
        Task SimulateTimeBasedChangesAsync();
    }

    public class InventoryService : IInventoryService
    {
        private readonly ApplicationDbContext _context;
        private static int _bookingCounter = 1000;

        public InventoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SeatInventoryDto?> GetInventoryAsync(string flightId)
        {
            var inventory = await _context.SeatInventories
                .FirstOrDefaultAsync(i => i.FlightId == flightId);

            if (inventory == null) return null;

            return new SeatInventoryDto
            {
                FlightId = inventory.FlightId,
                TotalSeats = inventory.TotalSeats,
                BookedSeats = inventory.BookedSeats,
                AvailableSeats = inventory.AvailableSeats,
                LastUpdated = inventory.LastUpdated
            };
        }

        public async Task<List<SeatInventoryDto>> GetAllInventoryAsync()
        {
            return await _context.SeatInventories
                .Select(i => new SeatInventoryDto
                {
                    FlightId = i.FlightId,
                    TotalSeats = i.TotalSeats,
                    BookedSeats = i.BookedSeats,
                    AvailableSeats = i.AvailableSeats,
                    LastUpdated = i.LastUpdated
                })
                .ToListAsync();
        }

        public async Task<BookingResponseDto> BookSeatsAsync(string flightId, string passengerName, int seats)
        {
            var inventory = await _context.SeatInventories
                .FirstOrDefaultAsync(i => i.FlightId == flightId);

            _bookingCounter++;

            if (inventory == null)
            {
                return new BookingResponseDto
                {
                    BookingId = $"BK{_bookingCounter - 1}",
                    FlightId = flightId,
                    PassengerName = passengerName,
                    SeatsBooked = 0,
                    Status = "failed"
                };
            }

            if (inventory.AvailableSeats >= seats)
            {
                inventory.AvailableSeats -= seats;
                inventory.BookedSeats += seats;
                inventory.LastUpdated = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return new BookingResponseDto
                {
                    BookingId = $"BK{_bookingCounter - 1}",
                    FlightId = flightId,
                    PassengerName = passengerName,
                    SeatsBooked = seats,
                    Status = "confirmed"
                };
            }

            return new BookingResponseDto
            {
                BookingId = $"BK{_bookingCounter - 1}",
                FlightId = flightId,
                PassengerName = passengerName,
                SeatsBooked = 0,
                Status = "failed"
            };
        }

        public async Task<bool> CancelSeatsAsync(string flightId, int seats)
        {
            var inventory = await _context.SeatInventories
                .FirstOrDefaultAsync(i => i.FlightId == flightId);

            if (inventory == null) return false;

            if (inventory.BookedSeats >= seats)
            {
                inventory.AvailableSeats += seats;
                inventory.BookedSeats -= seats;
                inventory.LastUpdated = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<double?> GetOccupancyRateAsync(string flightId)
        {
            var inventory = await _context.SeatInventories
                .FirstOrDefaultAsync(i => i.FlightId == flightId);

            if (inventory == null || inventory.TotalSeats == 0)
                return null;

            return Math.Round((double)inventory.BookedSeats / inventory.TotalSeats * 100, 2);
        }

        public async Task InitializeInventoryAsync(List<Flight> flights)
        {
            foreach (var flight in flights)
            {
                var existing = await _context.SeatInventories
                    .FirstOrDefaultAsync(i => i.FlightId == flight.FlightId);

                if (existing == null)
                {
                    var inventory = new SeatInventory
                    {
                        FlightId = flight.FlightId,
                        TotalSeats = flight.TotalSeats,
                        BookedSeats = flight.TotalSeats - flight.AvailableSeats,
                        AvailableSeats = flight.AvailableSeats,
                        LastUpdated = DateTime.UtcNow
                    };

                    await _context.SeatInventories.AddAsync(inventory);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task SimulateTimeBasedChangesAsync()
        {
            var inventories = await _context.SeatInventories.ToListAsync();
            var random = new Random();

            foreach (var inventory in inventories)
            {
                var change = random.Next(-2, 5);
                var newAvailable = inventory.AvailableSeats + change;
                newAvailable = Math.Max(0, Math.Min(newAvailable, inventory.TotalSeats - inventory.BookedSeats));
                inventory.AvailableSeats = newAvailable;
                inventory.LastUpdated = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
    }
}
