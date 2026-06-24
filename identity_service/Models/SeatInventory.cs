using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityService.Models
{
    public class SeatInventory
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("Flight")]
        public string FlightId { get; set; } = string.Empty;

        public Flight? Flight { get; set; }

        [Required]
        public int TotalSeats { get; set; }

        [Required]
        public int BookedSeats { get; set; }

        [Required]
        public int AvailableSeats { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    public class SeatInventoryDto
    {
        public string FlightId { get; set; } = string.Empty;
        public int TotalSeats { get; set; }
        public int BookedSeats { get; set; }
        public int AvailableSeats { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class BookingRequestDto
    {
        [Required]
        public string FlightId { get; set; } = string.Empty;

        [Required]
        public string PassengerName { get; set; } = string.Empty;

        [Required]
        public int SeatsRequested { get; set; }
    }

    public class BookingResponseDto
    {
        public string BookingId { get; set; } = string.Empty;
        public string FlightId { get; set; } = string.Empty;
        public string PassengerName { get; set; } = string.Empty;
        public int SeatsBooked { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class OccupancyDto
    {
        public string FlightId { get; set; } = string.Empty;
        public double OccupancyRate { get; set; }
        public string Unit { get; set; } = "%";
    }
}
