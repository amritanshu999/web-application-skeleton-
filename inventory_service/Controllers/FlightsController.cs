using Microsoft.AspNetCore.Mvc;
using InventoryService.Models;
using InventoryService.Services;

namespace InventoryService.Controllers
{
    [ApiController]
    [Route("flights")]
    public class FlightsController : ControllerBase
    {
        private readonly IFlightService _flightService;

        public FlightsController(IFlightService flightService)
        {
            _flightService = flightService;
        }

        [HttpGet]
        public async Task<ActionResult<List<FlightDto>>> GetAll()
        {
            var flights = await _flightService.GetAllFlightsAsync();
            return Ok(flights);
        }

        [HttpGet("{flightId}")]
        public async Task<ActionResult<FlightDto>> GetById(string flightId)
        {
            var flight = await _flightService.GetFlightByIdAsync(flightId);
            if (flight == null) return NotFound(new { message = "Flight not found" });
            return Ok(flight);
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<FlightDto>>> Search(
            [FromQuery] string departure,
            [FromQuery] string arrival)
        {
            if (string.IsNullOrWhiteSpace(departure) || string.IsNullOrWhiteSpace(arrival))
                return BadRequest(new { message = "departure and arrival query params required" });

            var flights = await _flightService.SearchFlightsAsync(departure, arrival);
            if (!flights.Any())
                return NotFound(new { message = $"No flights found for {departure.ToUpper()} → {arrival.ToUpper()}" });

            return Ok(flights);
        }

        [HttpGet("inventory")]
        public async Task<ActionResult<List<InventoryDto>>> GetInventory()
        {
            var inventory = await _flightService.GetInventoryAsync();
            return Ok(inventory);
        }
    }
}
