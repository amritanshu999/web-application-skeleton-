using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace InventoryService.Models
{
    public class Flight
    {
        [BsonId][BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("flightId")]
        public string FlightId { get; set; } = string.Empty;

        [BsonElement("flightNumber")]
        public string FlightNumber { get; set; } = string.Empty;

        [BsonElement("airline")]
        public string Airline { get; set; } = "IndiGo";

        [BsonElement("departure")]
        public AirportInfo Departure { get; set; } = new();

        [BsonElement("arrival")]
        public AirportInfo Arrival { get; set; } = new();

        [BsonElement("departureTime")]
        public string DepartureTime { get; set; } = string.Empty;

        [BsonElement("arrivalTime")]
        public string ArrivalTime { get; set; } = string.Empty;

        [BsonElement("durationMinutes")]
        public int DurationMinutes { get; set; }

        [BsonElement("aircraft")]
        public string Aircraft { get; set; } = "Airbus A320";

        [BsonElement("totalSeats")]
        public int TotalSeats { get; set; }

        [BsonElement("availableSeats")]
        public int AvailableSeats { get; set; }

        [BsonElement("bookedSeats")]
        public int BookedSeats { get; set; }

        [BsonElement("fare")]
        public FareInfo Fare { get; set; } = new();

        [BsonElement("status")]
        public string Status { get; set; } = "On Time";

        [BsonElement("lastUpdated")]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    public class AirportInfo
    {
        [BsonElement("code")]   public string Code     { get; set; } = string.Empty;
        [BsonElement("name")]   public string Name     { get; set; } = string.Empty;
        [BsonElement("city")]   public string City     { get; set; } = string.Empty;
        [BsonElement("terminal")] public string Terminal { get; set; } = string.Empty;
    }

    public class FareInfo
    {
        [BsonElement("economy")]  public int Economy  { get; set; }
        [BsonElement("business")] public int Business { get; set; }
    }

    // DTOs (no Bson attributes needed)
    public class FlightDto
    {
        public string FlightId { get; set; } = string.Empty;
        public string FlightNumber { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public AirportInfo Departure { get; set; } = new();
        public AirportInfo Arrival { get; set; } = new();
        public string DepartureTime { get; set; } = string.Empty;
        public string ArrivalTime { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public string Aircraft { get; set; } = string.Empty;
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public int BookedSeats { get; set; }
        public FareInfo Fare { get; set; } = new();
        public string Status { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; }
    }

    public class InventoryDto
    {
        public string FlightId { get; set; } = string.Empty;
        public string FlightNumber { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public int BookedSeats { get; set; }
        public double OccupancyRate { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
