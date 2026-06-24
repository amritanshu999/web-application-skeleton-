using Microsoft.AspNetCore.Mvc;
using BookingService.Models;
using BookingService.Services;

namespace BookingService.Controllers
{
    [ApiController]
    [Route("bookings")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _svc;
        public BookingsController(IBookingService svc) => _svc = svc;

        // POST /bookings  – create booking (seats reserved, awaiting payment)
        [HttpPost]
        public async Task<ActionResult<BookingResponseDto>> Create([FromBody] CreateBookingDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FlightId) ||
                string.IsNullOrWhiteSpace(dto.PassengerName) ||
                string.IsNullOrWhiteSpace(dto.PassengerEmail) ||
                dto.SeatsRequested < 1)
                return BadRequest(new { message = "flightId, passengerName, passengerEmail and seatsRequested are required." });

            var (result, error) = await _svc.CreateBookingAsync(dto);
            if (result == null) return BadRequest(new { message = error });
            return Ok(result);
        }

        // GET /bookings/{id}
        [HttpGet("{bookingId}")]
        public async Task<ActionResult> GetById(string bookingId)
        {
            var b = await _svc.GetBookingAsync(bookingId);
            if (b == null) return NotFound(new { message = "Booking not found." });
            return Ok(b);
        }

        // GET /bookings/by-email/{email}
        [HttpGet("by-email/{email}")]
        public async Task<ActionResult> GetByEmail(string email)
        {
            var list = await _svc.GetBookingsByEmailAsync(email);
            return Ok(list);
        }

        // DELETE /bookings/{id}  – cancel
        [HttpDelete("{bookingId}")]
        public async Task<ActionResult> Cancel(string bookingId)
        {
            var ok = await _svc.CancelBookingAsync(bookingId);
            if (!ok) return BadRequest(new { message = "Cannot cancel – booking not found or already cancelled." });
            return Ok(new { status = "cancelled", bookingId });
        }
    }
}
