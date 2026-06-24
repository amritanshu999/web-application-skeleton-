using Microsoft.AspNetCore.Mvc;
using BookingService.Models;
using BookingService.Services;

namespace BookingService.Controllers
{
    [ApiController]
    [Route("payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly IBookingService _svc;
        public PaymentsController(IBookingService svc) => _svc = svc;

        // POST /payments/confirm
        [HttpPost("confirm")]
        public async Task<ActionResult<PaymentResponseDto>> Confirm([FromBody] PaymentRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.BookingId) ||
                string.IsNullOrWhiteSpace(dto.PaymentMethod) ||
                string.IsNullOrWhiteSpace(dto.PaymentId))
                return BadRequest(new { message = "bookingId, paymentMethod and paymentId are required." });

            var (result, error) = await _svc.ProcessPaymentAsync(dto);
            if (result == null) return BadRequest(new { message = error });
            return Ok(result);
        }

        // GET /payments/qr-data/{bookingId}
        // Returns UPI deep-link data for QR generation on the frontend
        [HttpGet("qr-data/{bookingId}")]
        public async Task<ActionResult> GetQrData(string bookingId)
        {
            var booking = await _svc.GetBookingAsync(bookingId);
            if (booking == null) return NotFound(new { message = "Booking not found." });

            // Standard UPI payment string (apps like PhonePe / GPay scan this)
            var upiString =
                $"upi://pay?pa=indigo@upi&pn=IndiGo Airlines&am={booking.TotalFare}" +
                $"&tn=Flight {booking.FlightNumber} PNR {booking.PnrNumber}&cu=INR";

            return Ok(new
            {
                bookingId  = booking.BookingId,
                pnrNumber  = booking.PnrNumber,
                totalFare  = booking.TotalFare,
                upiString,
                upiId      = "indigo@upi",
                payeeName  = "IndiGo Airlines"
            });
        }
    }
}
