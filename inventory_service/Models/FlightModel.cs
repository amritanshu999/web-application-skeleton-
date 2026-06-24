namespace InventoryService.Models
{
    public class Flight
    {
        public string FlightId { get; set; } = string.Empty;
        public string FlightNumber { get; set; } = string.Empty;
        public string Airline { get; set; } = "IndiGo";
        public AirportInfo Departure { get; set; } = new();
        public AirportInfo Arrival { get; set; } = new();
        public string DepartureTime { get; set; } = string.Empty;
        public string ArrivalTime { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public string Aircraft { get; set; } = "Airbus A320";
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public int BookedSeats { get; set; }
        public FareInfo Fare { get; set; } = new();
        public string Status { get; set; } = "On Time";
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    public class AirportInfo
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Terminal { get; set; } = string.Empty;
    }

    public class FareInfo
    {
        public int Economy { get; set; }
        public int Business { get; set; }
    }

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
