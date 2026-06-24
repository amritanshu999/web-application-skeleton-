namespace BookingService.Models
{
    // ── Stored entity ─────────────────────────────────────────────────────────
    public class Booking
    {
        public string BookingId       { get; set; } = string.Empty;
        public string PnrNumber       { get; set; } = string.Empty;

        // Flight snapshot (copied at booking time so ticket is always accurate)
        public string FlightId        { get; set; } = string.Empty;
        public string FlightNumber    { get; set; } = string.Empty;
        public string DepartureCode   { get; set; } = string.Empty;
        public string ArrivalCode     { get; set; } = string.Empty;
        public string DepartureCity   { get; set; } = string.Empty;
        public string ArrivalCity     { get; set; } = string.Empty;
        public string DepartureName   { get; set; } = string.Empty;
        public string ArrivalName     { get; set; } = string.Empty;
        public string DepartureTerminal { get; set; } = string.Empty;
        public string ArrivalTerminal { get; set; } = string.Empty;
        public string DepartureTime   { get; set; } = string.Empty;
        public string ArrivalTime     { get; set; } = string.Empty;
        public string Aircraft        { get; set; } = string.Empty;

        // Passenger
        public string PassengerName   { get; set; } = string.Empty;
        public string PassengerEmail  { get; set; } = string.Empty;
        public int    SeatsBooked     { get; set; }
        public string SeatClass       { get; set; } = "economy";
        public int    TotalFare       { get; set; }

        // Status
        public string BookingStatus   { get; set; } = "pending_payment";  // pending_payment | confirmed | cancelled
        public string PaymentMethod   { get; set; } = string.Empty;       // qr | upi_id
        public string PaymentId       { get; set; } = string.Empty;
        public string PaymentStatus   { get; set; } = "pending";          // pending | paid

        public DateTime BookedAt      { get; set; } = DateTime.UtcNow;
        public DateTime? PaidAt       { get; set; }
    }

    // ── Request DTOs ──────────────────────────────────────────────────────────
    public class CreateBookingDto
    {
        public string FlightId       { get; set; } = string.Empty;
        public string PassengerName  { get; set; } = string.Empty;
        public string PassengerEmail { get; set; } = string.Empty;
        public int    SeatsRequested { get; set; } = 1;
        public string SeatClass      { get; set; } = "economy";
    }

    public class PaymentRequestDto
    {
        public string BookingId     { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty; // "qr" or "upi_id"
        public string PaymentId     { get; set; } = string.Empty; // UPI ID or QR ref
    }

    // ── Response DTOs ─────────────────────────────────────────────────────────
    public class BookingResponseDto
    {
        public string BookingId    { get; set; } = string.Empty;
        public string PnrNumber    { get; set; } = string.Empty;
        public string FlightId     { get; set; } = string.Empty;
        public string FlightNumber { get; set; } = string.Empty;
        public string PassengerName { get; set; } = string.Empty;
        public int    SeatsBooked  { get; set; }
        public string SeatClass    { get; set; } = string.Empty;
        public int    TotalFare    { get; set; }
        public string BookingStatus { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public DateTime BookedAt   { get; set; }
    }

    public class PaymentResponseDto
    {
        public string BookingId     { get; set; } = string.Empty;
        public string PnrNumber     { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string BookingStatus { get; set; } = string.Empty;
        public string Message       { get; set; } = string.Empty;
    }

    // ── Inventory service proxy model ─────────────────────────────────────────
    public class InventoryBookingRequest
    {
        public string FlightId       { get; set; } = string.Empty;
        public string PassengerName  { get; set; } = string.Empty;
        public string PassengerEmail { get; set; } = string.Empty;
        public int    SeatsRequested { get; set; }
        public string SeatClass      { get; set; } = string.Empty;
    }

    public class InventoryBookingResponse
    {
        public string BookingId    { get; set; } = string.Empty;
        public string FlightNumber { get; set; } = string.Empty;
        public int    SeatsBooked  { get; set; }
        public string SeatClass    { get; set; } = string.Empty;
        public int    TotalFare    { get; set; }
        public string Status       { get; set; } = string.Empty;
    }

    public class FlightInfo
    {
        public string FlightId     { get; set; } = string.Empty;
        public string FlightNumber { get; set; } = string.Empty;
        public AirportInfo Departure { get; set; } = new();
        public AirportInfo Arrival   { get; set; } = new();
        public string DepartureTime  { get; set; } = string.Empty;
        public string ArrivalTime    { get; set; } = string.Empty;
        public string Aircraft       { get; set; } = string.Empty;
        public FareInfo Fare         { get; set; } = new();
        public int AvailableSeats    { get; set; }
    }

    public class AirportInfo
    {
        public string Code     { get; set; } = string.Empty;
        public string Name     { get; set; } = string.Empty;
        public string City     { get; set; } = string.Empty;
        public string Terminal { get; set; } = string.Empty;
    }

    public class FareInfo
    {
        public int Economy  { get; set; }
        public int Business { get; set; }
    }
}
