using MongoDB.Driver;
using InventoryService.Data;
using InventoryService.Models;

namespace InventoryService.Services
{
    public interface IInventoryService
    {
        Task<BookingResponseDto> BookSeatsAsync(BookingRequestDto req);
        Task<bool> CancelBookingAsync(string bookingId);
        Task<List<Booking>> GetBookingsByEmailAsync(string email);
    }

    public class InventoryBookingService : IInventoryService
    {
        private readonly MongoDbContext _db;
        private static int _counter = 1000;

        public InventoryBookingService(MongoDbContext db) => _db = db;

        public async Task<BookingResponseDto> BookSeatsAsync(BookingRequestDto req)
        {
            string bookingId = $"BK{Interlocked.Increment(ref _counter)}";

            var flight = await _db.Flights.Find(f => f.FlightId == req.FlightId).FirstOrDefaultAsync();
            if (flight == null)
                return new BookingResponseDto { BookingId = bookingId, Status = "failed", FlightId = req.FlightId };

            if (flight.AvailableSeats < req.SeatsRequested)
                return new BookingResponseDto { BookingId = bookingId, Status = "failed",
                    FlightId = req.FlightId, FlightNumber = flight.FlightNumber };

            int unitFare  = req.SeatClass.Equals("business", StringComparison.OrdinalIgnoreCase)
                ? flight.Fare.Business : flight.Fare.Economy;
            int totalFare = unitFare * req.SeatsRequested;

            // Atomic seat update in MongoDB
            var update = Builders<Flight>.Update
                .Inc(f => f.AvailableSeats, -req.SeatsRequested)
                .Inc(f => f.BookedSeats,     req.SeatsRequested)
                .Set(f => f.LastUpdated, DateTime.UtcNow);
            await _db.Flights.UpdateOneAsync(f => f.FlightId == req.FlightId, update);

            var booking = new Booking
            {
                BookingId     = bookingId,
                FlightId      = req.FlightId,
                FlightNumber  = flight.FlightNumber,
                PassengerName  = req.PassengerName,
                PassengerEmail = req.PassengerEmail,
                SeatsBooked   = req.SeatsRequested,
                SeatClass     = req.SeatClass,
                TotalFare     = totalFare,
                Status        = "confirmed",
                BookedAt      = DateTime.UtcNow
            };
            await _db.Bookings.InsertOneAsync(booking);

            return new BookingResponseDto
            {
                BookingId = bookingId, FlightId = req.FlightId,
                FlightNumber = flight.FlightNumber, PassengerName = req.PassengerName,
                SeatsBooked = req.SeatsRequested, SeatClass = req.SeatClass,
                TotalFare = totalFare, Status = "confirmed", BookedAt = booking.BookedAt
            };
        }

        public async Task<bool> CancelBookingAsync(string bookingId)
        {
            var booking = await _db.Bookings.Find(b => b.BookingId == bookingId).FirstOrDefaultAsync();
            if (booking == null || booking.Status == "cancelled") return false;

            var update = Builders<Flight>.Update
                .Inc(f => f.AvailableSeats,  booking.SeatsBooked)
                .Inc(f => f.BookedSeats,     -booking.SeatsBooked)
                .Set(f => f.LastUpdated, DateTime.UtcNow);
            await _db.Flights.UpdateOneAsync(f => f.FlightId == booking.FlightId, update);

            await _db.Bookings.UpdateOneAsync(b => b.BookingId == bookingId,
                Builders<Booking>.Update.Set(b => b.Status, "cancelled"));
            return true;
        }

        public async Task<List<Booking>> GetBookingsByEmailAsync(string email)
            => await _db.Bookings
                .Find(b => b.PassengerEmail == email)
                .SortByDescending(b => b.BookedAt)
                .ToListAsync();
    }
}
