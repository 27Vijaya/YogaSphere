using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YogaSphere.Application.DTOs.Payment;
using YogaSphere.Application.Interfaces;

namespace YogaSphere.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Customer")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // Initiates payment for the authenticated customer's booking.
        [HttpPost("initiate")]
        public async Task<IActionResult> Initiate(
            [FromBody] CreatePaymentRequest request)
        {
            try
            {
                var customerId = GetCustomerId();

                var response = await _paymentService.InitiateAsync(
                    customerId, request);

                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Retrieves a payment belonging to the authenticated customer.
        [HttpGet("{paymentId:int}")]
        public async Task<IActionResult> GetById(int paymentId)
        {
            var customerId = GetCustomerId();

            var response = await _paymentService.GetByIdAsync(
                customerId, paymentId);

            if (response == null)
                return NotFound();

            return Ok(response);
        }

        // Retrieves payment history for one of the customer's bookings.
        [HttpGet("booking/{bookingId:int}")]
        public async Task<IActionResult> GetByBookingId(int bookingId)
        {
            var customerId = GetCustomerId();

            var response = await _paymentService.GetByBookingIdAsync(
                customerId, bookingId);

            return Ok(response);
        }

        // Reads the authenticated user's ID from the JWT token.
        private int GetCustomerId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out var customerId))
                throw new UnauthorizedAccessException(
                    "Invalid customer identity.");

            return customerId;
        }
    }
}