using InventoryService.Data;
using InventoryService.Models;

namespace InventoryService.Services
{
    public interface IInventoryService
    {
        Task<BookingResponseDto> BookSeatsAsync(BookingRequestDto request);
        Task<bool> CancelBookingAsync(string bookingId);
        Task<List<Booking>> GetBookingsByEmailAsync(string email);
    }

    public class InventoryBookingService : IInventoryService
    {
        private readonly JsonDbContext _db;
        private static int _counter = 1000;

        public InventoryBookingService(JsonDbContext db) => _db = db;

        public async Task<BookingResponseDto> BookSeatsAsync(BookingRequestDto req)
        {
            string bookingId = $"BK{Interlocked.Increment(ref _counter)}";

            var flight = await _db.GetFlightAsync(req.FlightId);
            if (flight == null)
                return new BookingResponseDto { BookingId = bookingId, Status = "failed", FlightId = req.FlightId };

            if (flight.AvailableSeats < req.SeatsRequested)
                return new BookingResponseDto { BookingId = bookingId, Status = "failed",
                    FlightId = req.FlightId, FlightNumber = flight.FlightNumber };

            int unitFare  = req.SeatClass.Equals("business", StringComparison.OrdinalIgnoreCase)
                ? flight.Fare.Business : flight.Fare.Economy;
            int totalFare = unitFare * req.SeatsRequested;

            // Update seats – persisted to disk immediately
            await _db.UpdateFlightSeatsAsync(req.FlightId, -req.SeatsRequested, req.SeatsRequested);

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
            await _db.AddBookingAsync(booking);

            return new BookingResponseDto
            {
                BookingId    = bookingId, FlightId = req.FlightId,
                FlightNumber = flight.FlightNumber, PassengerName = req.PassengerName,
                SeatsBooked  = req.SeatsRequested, SeatClass = req.SeatClass,
                TotalFare    = totalFare, Status = "confirmed", BookedAt = booking.BookedAt
            };
        }

        public async Task<bool> CancelBookingAsync(string bookingId)
        {
            var booking = await _db.GetBookingAsync(bookingId);
            if (booking == null || booking.Status == "cancelled") return false;

            await _db.UpdateFlightSeatsAsync(booking.FlightId, booking.SeatsBooked, -booking.SeatsBooked);
            await _db.UpdateBookingStatusAsync(bookingId, "cancelled");
            return true;
        }

        public Task<List<Booking>> GetBookingsByEmailAsync(string email)
            => _db.GetBookingsByEmailAsync(email);
    }
}
