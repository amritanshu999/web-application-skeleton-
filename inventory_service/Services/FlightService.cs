using InventoryService.Data;
using InventoryService.Models;

namespace InventoryService.Services
{
    public interface IFlightService
    {
        Task<List<FlightDto>> GetAllFlightsAsync();
        Task<FlightDto?> GetFlightByIdAsync(string flightId);
        Task<List<FlightDto>> SearchFlightsAsync(string departure, string arrival);
        Task<List<InventoryDto>> GetInventoryAsync();
    }

    public class FlightService : IFlightService
    {
        private readonly JsonDbContext _db;
        public FlightService(JsonDbContext db) => _db = db;

        public async Task<List<FlightDto>> GetAllFlightsAsync()
            => (await _db.GetFlightsAsync()).Select(ToDto).ToList();

        public async Task<FlightDto?> GetFlightByIdAsync(string flightId)
        {
            var f = await _db.GetFlightAsync(flightId);
            return f == null ? null : ToDto(f);
        }

        public async Task<List<FlightDto>> SearchFlightsAsync(string dep, string arr)
            => (await _db.SearchFlightsAsync(dep, arr)).Select(ToDto).ToList();

        public async Task<List<InventoryDto>> GetInventoryAsync()
            => (await _db.GetFlightsAsync()).Select(f => new InventoryDto
            {
                FlightId       = f.FlightId,
                FlightNumber   = f.FlightNumber,
                Route          = $"{f.Departure.Code} → {f.Arrival.Code}",
                TotalSeats     = f.TotalSeats,
                AvailableSeats = f.AvailableSeats,
                BookedSeats    = f.BookedSeats,
                OccupancyRate  = f.TotalSeats > 0 ? Math.Round((double)f.BookedSeats / f.TotalSeats * 100, 1) : 0,
                LastUpdated    = f.LastUpdated
            }).ToList();

        private static FlightDto ToDto(Flight f) => new()
        {
            FlightId = f.FlightId, FlightNumber = f.FlightNumber, Airline = f.Airline,
            Departure = f.Departure, Arrival = f.Arrival,
            DepartureTime = f.DepartureTime, ArrivalTime = f.ArrivalTime,
            DurationMinutes = f.DurationMinutes, Aircraft = f.Aircraft,
            TotalSeats = f.TotalSeats, AvailableSeats = f.AvailableSeats,
            BookedSeats = f.BookedSeats, Fare = f.Fare,
            Status = f.Status, LastUpdated = f.LastUpdated
        };
    }
}
