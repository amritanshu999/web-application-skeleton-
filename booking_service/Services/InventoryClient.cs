using System.Net.Http.Json;
using BookingService.Models;

namespace BookingService.Services
{
    /// <summary>
    /// HTTP client that talks to inventory_service (localhost:5001).
    /// Fetches flight info and reserves seats.
    /// </summary>
    public class InventoryClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<InventoryClient> _log;

        public InventoryClient(HttpClient http, ILogger<InventoryClient> log)
        {
            _http = http;
            _log  = log;
        }

        public async Task<FlightInfo?> GetFlightAsync(string flightId)
        {
            try
            {
                return await _http.GetFromJsonAsync<FlightInfo>($"/flights/{flightId}");
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Failed to fetch flight {FlightId} from inventory_service", flightId);
                return null;
            }
        }

        public async Task<InventoryBookingResponse?> ReserveSeatsAsync(InventoryBookingRequest req)
        {
            try
            {
                var resp = await _http.PostAsJsonAsync("/bookings", req);
                if (!resp.IsSuccessStatusCode) return null;
                return await resp.Content.ReadFromJsonAsync<InventoryBookingResponse>();
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Failed to reserve seats on inventory_service");
                return null;
            }
        }

        public async Task<bool> CancelReservationAsync(string inventoryBookingId)
        {
            try
            {
                var resp = await _http.DeleteAsync($"/bookings/{inventoryBookingId}");
                return resp.IsSuccessStatusCode;
            }
            catch { return false; }
        }
    }
}
