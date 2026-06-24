using IdentityService.Data;
using IdentityService.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Services
{
    public interface IFlightService
    {
        Task<List<FlightDto>> GetAllFlightsAsync();
        Task<FlightDto?> GetFlightByIdAsync(string flightId);
        Task<List<FlightDto>> SearchFlightsAsync(string departure, string arrival);
        Task InitializeDummyFlightsAsync();
    }

    public class FlightService : IFlightService
    {
        private readonly ApplicationDbContext _context;

        public FlightService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<FlightDto>> GetAllFlightsAsync()
        {
            return await _context.Flights
                .Select(f => new FlightDto
                {
                    FlightId = f.FlightId,
                    Airline = f.Airline,
                    Departure = f.Departure,
                    Arrival = f.Arrival,
                    DepartureTime = f.DepartureTime,
                    ArrivalTime = f.ArrivalTime,
                    TotalSeats = f.TotalSeats,
                    AvailableSeats = f.AvailableSeats
                })
                .ToListAsync();
        }

        public async Task<FlightDto?> GetFlightByIdAsync(string flightId)
        {
            var flight = await _context.Flights.FirstOrDefaultAsync(f => f.FlightId == flightId);
            if (flight == null) return null;

            return new FlightDto
            {
                FlightId = flight.FlightId,
                Airline = flight.Airline,
                Departure = flight.Departure,
                Arrival = flight.Arrival,
                DepartureTime = flight.DepartureTime,
                ArrivalTime = flight.ArrivalTime,
                TotalSeats = flight.TotalSeats,
                AvailableSeats = flight.AvailableSeats
            };
        }

        public async Task<List<FlightDto>> SearchFlightsAsync(string departure, string arrival)
        {
            return await _context.Flights
                .Where(f => f.Departure == departure && f.Arrival == arrival)
                .Select(f => new FlightDto
                {
                    FlightId = f.FlightId,
                    Airline = f.Airline,
                    Departure = f.Departure,
                    Arrival = f.Arrival,
                    DepartureTime = f.DepartureTime,
                    ArrivalTime = f.ArrivalTime,
                    TotalSeats = f.TotalSeats,
                    AvailableSeats = f.AvailableSeats
                })
                .ToListAsync();
        }

        public async Task InitializeDummyFlightsAsync()
        {
            // Check if flights already exist
            if (await _context.Flights.AnyAsync())
                return;

            var baseTime = DateTime.Now.Date;
            var random = new Random();
            var flightsData = new[]
            {
                new Flight
                {
                    FlightId = "IG001",
                    Airline = "Indigo",
                    Departure = "DEL",
                    Arrival = "BOM",
                    DepartureTime = baseTime.AddHours(8),
                    ArrivalTime = baseTime.AddHours(10).AddMinutes(30),
                    TotalSeats = 180,
                    AvailableSeats = random.Next(10, 180)
                },
                new Flight
                {
                    FlightId = "IG002",
                    Airline = "Indigo",
                    Departure = "BOM",
                    Arrival = "BLR",
                    DepartureTime = baseTime.AddHours(11),
                    ArrivalTime = baseTime.AddHours(13).AddMinutes(15),
                    TotalSeats = 150,
                    AvailableSeats = random.Next(10, 150)
                },
                new Flight
                {
                    FlightId = "IG003",
                    Airline = "Indigo",
                    Departure = "BLR",
                    Arrival = "HYD",
                    DepartureTime = baseTime.AddHours(14),
                    ArrivalTime = baseTime.AddHours(15).AddMinutes(30),
                    TotalSeats = 160,
                    AvailableSeats = random.Next(10, 160)
                },
                new Flight
                {
                    FlightId = "IG004",
                    Airline = "Indigo",
                    Departure = "DEL",
                    Arrival = "HYD",
                    DepartureTime = baseTime.AddHours(18),
                    ArrivalTime = baseTime.AddHours(20).AddMinutes(45),
                    TotalSeats = 180,
                    AvailableSeats = random.Next(10, 180)
                },
                new Flight
                {
                    FlightId = "IG005",
                    Airline = "Indigo",
                    Departure = "HYD",
                    Arrival = "DEL",
                    DepartureTime = baseTime.AddHours(21),
                    ArrivalTime = baseTime.AddHours(23).AddMinutes(30),
                    TotalSeats = 175,
                    AvailableSeats = random.Next(10, 175)
                }
            };

            await _context.Flights.AddRangeAsync(flightsData);
            await _context.SaveChangesAsync();
        }
    }
}
