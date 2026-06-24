using Microsoft.AspNetCore.Mvc;
using IdentityService.Models;
using IdentityService.Services;

namespace IdentityService.Controllers
{
    [ApiController]
    [Route("inventory")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly IFlightService _flightService;

        public InventoryController(IInventoryService inventoryService, IFlightService flightService)
        {
            _inventoryService = inventoryService;
            _flightService = flightService;
        }

        [HttpGet]
        public async Task<ActionResult<List<SeatInventoryDto>>> GetAllInventory()
        {
            var inventory = await _inventoryService.GetAllInventoryAsync();
            return Ok(inventory);
        }

        [HttpGet("{flightId}")]
        public async Task<ActionResult<SeatInventoryDto>> GetInventory(string flightId)
        {
            var inventory = await _inventoryService.GetInventoryAsync(flightId);
            if (inventory == null)
                return NotFound(new { message = "Flight not found" });

            return Ok(inventory);
        }

        [HttpPost("book")]
        public async Task<ActionResult<BookingResponseDto>> BookSeats([FromBody] BookingRequestDto bookingRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _inventoryService.BookSeatsAsync(
                bookingRequest.FlightId,
                bookingRequest.PassengerName,
                bookingRequest.SeatsRequested);

            if (response.Status == "failed")
                return BadRequest(new { message = "Booking failed - insufficient seats available" });

            return Ok(response);
        }

        [HttpPost("cancel/{flightId}")]
        public async Task<ActionResult> CancelBooking(string flightId, [FromQuery] int seats = 1)
        {
            var success = await _inventoryService.CancelSeatsAsync(flightId, seats);
            if (!success)
                return BadRequest(new { message = "Cancellation failed" });

            return Ok(new { status = "success", message = $"{seats} seats returned for flight {flightId}" });
        }

        [HttpGet("occupancy/{flightId}")]
        public async Task<ActionResult<OccupancyDto>> GetOccupancy(string flightId)
        {
            var occupancy = await _inventoryService.GetOccupancyRateAsync(flightId);
            if (occupancy == null)
                return NotFound(new { message = "Flight not found" });

            return Ok(new OccupancyDto
            {
                FlightId = flightId,
                OccupancyRate = occupancy.Value
            });
        }

        [HttpPost("simulate")]
        public async Task<ActionResult> SimulateUpdates()
        {
            await _inventoryService.SimulateTimeBasedChangesAsync();
            return Ok(new { status = "success", message = "Seat availability update simulation completed" });
        }
    }
}
