using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YogaSphere.Application.DTOs.Booking;
using YogaSphere.Application.Interfaces;

namespace YogaSphere.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Customer")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // Retrieves the authenticated customer's bookings.
        [HttpGet("my-bookings")]
        public async Task<IActionResult> GetMyBookings()
        {
            var customerId = GetCustomerId();

            if (customerId == null)
                return Unauthorized();

            var bookings = await _bookingService
                .GetMyBookingsAsync(customerId.Value);

            return Ok(bookings);
        }

        // Retrieves a booking only if it belongs to the authenticated customer.
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customerId = GetCustomerId();

            if (customerId == null)
                return Unauthorized();

            var bookings = await _bookingService
                .GetMyBookingsAsync(customerId.Value);

            var booking = bookings.FirstOrDefault(
                booking => booking.Id == id);

            if (booking == null)
                return NotFound();

            return Ok(booking);
        }

        // Creates a booking for the authenticated customer.
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateBookingRequest request)
        {
            var customerId = GetCustomerId();

            if (customerId == null)
                return Unauthorized();

            try
            {
                var booking = await _bookingService.CreateAsync(
                    customerId.Value, request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = booking.Id },
                    booking);
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new { message = exception.Message });
            }
        }

        // Cancels a booking belonging to the authenticated customer.
        [HttpPut("{id:int}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var customerId = GetCustomerId();

            if (customerId == null)
                return Unauthorized();

            try
            {
                var cancelled = await _bookingService.CancelAsync(
                    id, customerId.Value);

                if (!cancelled)
                    return NotFound();

                return NoContent();
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new { message = exception.Message });
            }
        }

        // Reads the user ID from the authenticated JWT token.
        private int? GetCustomerId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(userId, out var customerId)
                ? customerId
                : null;
        }
    }
}