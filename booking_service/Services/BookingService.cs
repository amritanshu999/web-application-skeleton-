using BookingService.Data;
using BookingService.Models;

namespace BookingService.Services
{
    public interface IBookingService
    {
        Task<(BookingResponseDto? dto, string error)> CreateBookingAsync(CreateBookingDto req);
        Task<(PaymentResponseDto? dto, string error)> ProcessPaymentAsync(PaymentRequestDto req);
        Task<Booking?> GetBookingAsync(string bookingId);
        Task<List<Booking>> GetBookingsByEmailAsync(string email);
        Task<bool> CancelBookingAsync(string bookingId);
    }

    public class BookingServiceImpl : IBookingService
    {
        private readonly BookingDbContext  _db;
        private readonly InventoryClient   _inventory;
        private static int _counter = 2000;

        public BookingServiceImpl(BookingDbContext db, InventoryClient inventory)
        {
            _db        = db;
            _inventory = inventory;
        }

        // ── Step 1: Create booking (holds seat, awaits payment) ───────────────

        public async Task<(BookingResponseDto? dto, string error)> CreateBookingAsync(CreateBookingDto req)
        {
            // Fetch flight details from inventory_service
            var flight = await _inventory.GetFlightAsync(req.FlightId);
            if (flight == null)
                return (null, "Flight not found or inventory service unavailable.");

            if (flight.AvailableSeats < req.SeatsRequested)
                return (null, $"Only {flight.AvailableSeats} seats available.");

            // Reserve seats in inventory_service
            var invResp = await _inventory.ReserveSeatsAsync(new InventoryBookingRequest
            {
                FlightId       = req.FlightId,
                PassengerName  = req.PassengerName,
                PassengerEmail = req.PassengerEmail,
                SeatsRequested = req.SeatsRequested,
                SeatClass      = req.SeatClass
            });

            if (invResp == null || invResp.Status == "failed")
                return (null, "Seat reservation failed.");

            string bookingId = $"BK{Interlocked.Increment(ref _counter)}";
            string pnr       = GeneratePnr();

            var booking = new Booking
            {
                BookingId         = bookingId,
                PnrNumber         = pnr,
                FlightId          = flight.FlightId,
                FlightNumber      = flight.FlightNumber,
                DepartureCode     = flight.Departure.Code,
                ArrivalCode       = flight.Arrival.Code,
                DepartureCity     = flight.Departure.City,
                ArrivalCity       = flight.Arrival.City,
                DepartureName     = flight.Departure.Name,
                ArrivalName       = flight.Arrival.Name,
                DepartureTerminal = flight.Departure.Terminal,
                ArrivalTerminal   = flight.Arrival.Terminal,
                DepartureTime     = flight.DepartureTime,
                ArrivalTime       = flight.ArrivalTime,
                Aircraft          = flight.Aircraft,
                PassengerName     = req.PassengerName,
                PassengerEmail    = req.PassengerEmail,
                SeatsBooked       = req.SeatsRequested,
                SeatClass         = req.SeatClass,
                TotalFare         = invResp.TotalFare,
                BookingStatus     = "pending_payment",
                PaymentStatus     = "pending",
                BookedAt          = DateTime.UtcNow
            };

            await _db.AddAsync(booking);

            return (new BookingResponseDto
            {
                BookingId     = bookingId,
                PnrNumber     = pnr,
                FlightId      = booking.FlightId,
                FlightNumber  = booking.FlightNumber,
                PassengerName = booking.PassengerName,
                SeatsBooked   = booking.SeatsBooked,
                SeatClass     = booking.SeatClass,
                TotalFare     = booking.TotalFare,
                BookingStatus = booking.BookingStatus,
                PaymentStatus = booking.PaymentStatus,
                BookedAt      = booking.BookedAt
            }, string.Empty);
        }

        // ── Step 2: Process payment → confirm booking ─────────────────────────

        public async Task<(PaymentResponseDto? dto, string error)> ProcessPaymentAsync(PaymentRequestDto req)
        {
            var booking = await _db.GetAsync(req.BookingId);
            if (booking == null)
                return (null, "Booking not found.");

            if (booking.BookingStatus == "cancelled")
                return (null, "Booking is already cancelled.");

            if (booking.PaymentStatus == "paid")
                return (new PaymentResponseDto
                {
                    BookingId     = booking.BookingId,
                    PnrNumber     = booking.PnrNumber,
                    PaymentStatus = "paid",
                    BookingStatus = "confirmed",
                    Message       = "Payment already completed."
                }, string.Empty);

            // Validate payment method
            if (string.IsNullOrWhiteSpace(req.PaymentMethod) || string.IsNullOrWhiteSpace(req.PaymentId))
                return (null, "Payment method and payment ID are required.");

            // Simulate payment verification
            // In production, call a real payment gateway here.
            bool paymentSuccess = SimulatePaymentGateway(req.PaymentMethod, req.PaymentId);
            if (!paymentSuccess)
                return (null, "Payment verification failed. Check your UPI ID or QR reference.");

            booking.PaymentMethod = req.PaymentMethod;
            booking.PaymentId     = req.PaymentId;
            booking.PaymentStatus = "paid";
            booking.BookingStatus = "confirmed";
            booking.PaidAt        = DateTime.UtcNow;

            await _db.UpdateAsync(booking);

            return (new PaymentResponseDto
            {
                BookingId     = booking.BookingId,
                PnrNumber     = booking.PnrNumber,
                PaymentStatus = "paid",
                BookingStatus = "confirmed",
                Message       = "Payment successful. Your ticket is confirmed!"
            }, string.Empty);
        }

        public Task<Booking?> GetBookingAsync(string bookingId)
            => _db.GetAsync(bookingId);

        public Task<List<Booking>> GetBookingsByEmailAsync(string email)
            => _db.GetByEmailAsync(email);

        public async Task<bool> CancelBookingAsync(string bookingId)
        {
            var booking = await _db.GetAsync(bookingId);
            if (booking == null || booking.BookingStatus == "cancelled") return false;

            booking.BookingStatus = "cancelled";
            await _db.UpdateAsync(booking);

            // Also release seat back in inventory_service
            await _inventory.CancelReservationAsync(bookingId);
            return true;
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static string GeneratePnr()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var rnd = new Random();
            return new string(Enumerable.Range(0, 6).Select(_ => chars[rnd.Next(chars.Length)]).ToArray());
        }

        /// <summary>
        /// Simulates payment gateway verification.
        /// Accepts any non-empty UPI ID that contains '@' or any 12+ digit QR ref.
        /// </summary>
        private static bool SimulatePaymentGateway(string method, string paymentId)
        {
            return method.ToLower() switch
            {
                "upi_id" => paymentId.Contains('@') && paymentId.Length >= 6,
                "qr"     => paymentId.Length >= 8,
                _        => false
            };
        }
    }
}
