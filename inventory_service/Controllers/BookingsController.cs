using Microsoft.AspNetCore.Mvc;
using InventoryService.Models;
using InventoryService.Services;

namespace InventoryService.Controllers
{
    [ApiController]
    [Route("bookings")]
    public class BookingsController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public BookingsController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpPost]
        public async Task<ActionResult<BookingResponseDto>> Book([FromBody] BookingRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.FlightId) ||
                string.IsNullOrWhiteSpace(request.PassengerName) ||
                request.SeatsRequested < 1)
                return BadRequest(new { message = "flightId, passengerName and seatsRequested are required" });

            var result = await _inventoryService.BookSeatsAsync(request);

            if (result.Status == "failed")
                return BadRequest(new { message = "Booking failed – insufficient seats or flight not found" });

            return Ok(result);
        }

        [HttpDelete("{bookingId}")]
        public async Task<ActionResult> Cancel(string bookingId)
        {
            var ok = await _inventoryService.CancelBookingAsync(bookingId);
            if (!ok) return BadRequest(new { message = "Cancellation failed – booking not found or already cancelled" });
            return Ok(new { status = "cancelled", bookingId });
        }

        [HttpGet("by-email/{email}")]
        public async Task<ActionResult> GetByEmail(string email)
        {
            var bookings = await _inventoryService.GetBookingsByEmailAsync(email);
            return Ok(bookings);
        }
    }
}
