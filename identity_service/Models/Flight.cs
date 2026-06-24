using System.ComponentModel.DataAnnotations;

namespace IdentityService.Models
{
    public class Flight
    {
        [Key]
        public string FlightId { get; set; } = string.Empty;

        [Required]
        public string Airline { get; set; } = string.Empty;

        [Required]
        public string Departure { get; set; } = string.Empty;

        [Required]
        public string Arrival { get; set; } = string.Empty;

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }

        [Required]
        public int TotalSeats { get; set; }

        [Required]
        public int AvailableSeats { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class FlightDto
    {
        public string FlightId { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public string Departure { get; set; } = string.Empty;
        public string Arrival { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
    }

    public class FlightCreateDto
    {
        [Required]
        public string Airline { get; set; } = string.Empty;

        [Required]
        public string Departure { get; set; } = string.Empty;

        [Required]
        public string Arrival { get; set; } = string.Empty;

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }

        [Required]
        public int TotalSeats { get; set; }
    }
}
