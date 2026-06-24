using Microsoft.AspNetCore.Mvc;
using IdentityService.Models;
using IdentityService.Services;

namespace IdentityService.Controllers
{
    [ApiController]
    [Route("flights")]
    public class FlightController : ControllerBase
    {
        private readonly IFlightService _flightService;

        public FlightController(IFlightService flightService)
        {
            _flightService = flightService;
        }

        [HttpGet]
        public async Task<ActionResult<List<FlightDto>>> GetAllFlights()
        {
            var flights = await _flightService.GetAllFlightsAsync();
            return Ok(flights);
        }

        [HttpGet("{flightId}")]
        public async Task<ActionResult<FlightDto>> GetFlight(string flightId)
        {
            var flight = await _flightService.GetFlightByIdAsync(flightId);
            if (flight == null)
                return NotFound(new { message = "Flight not found" });

            return Ok(flight);
        }

        [HttpGet("search/{departure}/{arrival}")]
        public async Task<ActionResult<List<FlightDto>>> SearchFlights(string departure, string arrival)
        {
            var flights = await _flightService.SearchFlightsAsync(departure, arrival);
            if (!flights.Any())
                return NotFound(new { message = "No flights found for this route" });

            return Ok(flights);
        }
    }
}
