using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace InventoryService.Models
{
    public class Booking
    {
        [BsonId][BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("bookingId")]    public string BookingId     { get; set; } = string.Empty;
        [BsonElement("flightId")]     public string FlightId      { get; set; } = string.Empty;
        [BsonElement("flightNumber")] public string FlightNumber  { get; set; } = string.Empty;
        [BsonElement("passengerName")] public string PassengerName { get; set; } = string.Empty;
        [BsonElement("passengerEmail")] public string PassengerEmail { get; set; } = string.Empty;
        [BsonElement("seatsBooked")]  public int    SeatsBooked   { get; set; }
        [BsonElement("seatClass")]    public string SeatClass     { get; set; } = "economy";
        [BsonElement("totalFare")]    public int    TotalFare     { get; set; }
        [BsonElement("status")]       public string Status        { get; set; } = "confirmed";
        [BsonElement("bookedAt")]     public DateTime BookedAt    { get; set; } = DateTime.UtcNow;
    }

    public class BookingRequestDto
    {
        public string FlightId      { get; set; } = string.Empty;
        public string PassengerName { get; set; } = string.Empty;
        public string PassengerEmail { get; set; } = string.Empty;
        public int    SeatsRequested { get; set; } = 1;
        public string SeatClass     { get; set; } = "economy";
    }

    public class BookingResponseDto
    {
        public string BookingId    { get; set; } = string.Empty;
        public string FlightId     { get; set; } = string.Empty;
        public string FlightNumber { get; set; } = string.Empty;
        public string PassengerName { get; set; } = string.Empty;
        public int    SeatsBooked  { get; set; }
        public string SeatClass    { get; set; } = string.Empty;
        public int    TotalFare    { get; set; }
        public string Status       { get; set; } = string.Empty;
        public DateTime BookedAt   { get; set; }
    }

    public class OccupancyDto
    {
        public string FlightId     { get; set; } = string.Empty;
        public double OccupancyRate { get; set; }
        public string Unit         { get; set; } = "%";
    }
}
