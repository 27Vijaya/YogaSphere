using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using YogaSphere.Application.Interfaces;

namespace YogaSphere.API.Controllers
{
    [ApiController]
    [Route("api/webhooks/razorpay")]
    public class RazorpayWebhookController : ControllerBase
    {
        private readonly IRazorpayService _razorpayService;
        private readonly IRazorpayWebhookService _webhookService;

        public RazorpayWebhookController(IRazorpayService razorpayService,IRazorpayWebhookService webhookService)
        {
            _razorpayService = razorpayService;
            _webhookService = webhookService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> HandleWebhook()
        {
            // Read the raw request body. Razorpay signs the exact payload.
            using var reader = new StreamReader(Request.Body);
            var payload = await reader.ReadToEndAsync();

            // Get the signature sent by Razorpay.
            var signature = Request.Headers["X-Razorpay-Signature"].ToString();

            // Reject requests with an invalid signature.
            if (!_razorpayService.VerifyWebhookSignature(payload, signature))
            {
                return Unauthorized();
            }

            try
            {
                // Process the event only after verifying Razorpay's signature.
                await _webhookService.ProcessWebhookAsync(payload);

                return Ok();
            }
            catch (JsonException)
            {
                // The webhook payload is malformed.
                return BadRequest("Invalid webhook payload.");
            }
            catch (InvalidOperationException ex)
            {
                // The webhook contains invalid payment information.
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                // Allow Razorpay to retry when an unexpected server error occurs.
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}