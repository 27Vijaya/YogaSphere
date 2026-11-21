using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using YogaSphere.Application.Interfaces;
using YogaSphere.Infrastructure.Configuration;

namespace YogaSphere.Infrastructure.Services
{
    public class RazorpayService : IRazorpayService
    {
        private readonly HttpClient _httpClient;
        private readonly RazorpaySettings _settings;

        public RazorpayService(HttpClient httpClient,IOptions<RazorpaySettings> options)
        {
            _httpClient = httpClient;
            _settings = options.Value;

            if (string.IsNullOrWhiteSpace(_settings.KeyId) ||
                string.IsNullOrWhiteSpace(_settings.KeySecret))
            {
                throw new InvalidOperationException(
                    "Razorpay KeyId or KeySecret is missing from configuration.");
            } 

            // Configure Razorpay API authentication.
            var credentials = Convert.ToBase64String( Encoding.UTF8.GetBytes(
                    $"{_settings.KeyId}:{_settings.KeySecret}"));

            _httpClient.BaseAddress =
                new Uri("https://api.razorpay.com/v1/");

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", credentials);
        }

        // Creates an order through Razorpay's Orders API.
        public async Task<(string OrderId, string KeyId)> CreateOrderAsync(
            decimal amount,string currency)
        {
            if (amount <= 0)
                throw new InvalidOperationException("Invalid payment amount.");

            // Razorpay expects the amount in the smallest currency unit (paise).
            var amountInPaise = (long)Math.Round(
                amount * 100,
                MidpointRounding.AwayFromZero);

            var orderRequest = new
            {
                amount = amountInPaise,
                currency = currency,
                receipt = $"ys_{Guid.NewGuid():N}"
            };

            var response = await _httpClient.PostAsJsonAsync(
                "orders",
                orderRequest);

            response.EnsureSuccessStatusCode();

            using var document = JsonDocument.Parse(
                await response.Content.ReadAsStringAsync());

            var orderId = document.RootElement
                .GetProperty("id")
                .GetString();

            if (string.IsNullOrWhiteSpace(orderId))
                throw new InvalidOperationException(
                    "Razorpay did not return an order ID.");

            return (orderId, _settings.KeyId);
        }

        // Verifies the signature returned by Razorpay Checkout.
        public bool VerifyPaymentSignature(string orderId,string paymentId,string signature)
        {
            if (string.IsNullOrWhiteSpace(orderId) ||
                string.IsNullOrWhiteSpace(paymentId) ||
                string.IsNullOrWhiteSpace(signature))
            {
                return false;
            }

            var payload = $"{orderId}|{paymentId}";

            using var hmac = new HMACSHA256(
                Encoding.UTF8.GetBytes(_settings.KeySecret));

            var hash = hmac.ComputeHash(
                Encoding.UTF8.GetBytes(payload));

            var expectedSignature = Convert.ToHexString(hash)
                .ToLowerInvariant();

            try
            {
                return CryptographicOperations.FixedTimeEquals(
                    Convert.FromHexString(expectedSignature),
                    Convert.FromHexString(signature));
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public async Task<(string OrderId, long Amount, string Currency, string Status, bool Captured)>
            GetPaymentDetailsAsync(string paymentId)
        {
            if (string.IsNullOrWhiteSpace(paymentId))
                throw new InvalidOperationException("Payment ID is required.");

            // Retrieve payment details directly from Razorpay.
            var response = await _httpClient.GetAsync($"payments/{paymentId}");

            // Handle unsuccessful responses from Razorpay.
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();

                throw new InvalidOperationException(
                    $"Razorpay payment verification failed: {errorResponse}");
            }

            using var document = JsonDocument.Parse(
                await response.Content.ReadAsStringAsync());

            var root = document.RootElement;

            return (
                root.GetProperty("order_id").GetString() ?? string.Empty,
                root.GetProperty("amount").GetInt64(),
                root.GetProperty("currency").GetString() ?? string.Empty,
                root.GetProperty("status").GetString() ?? string.Empty,
                root.GetProperty("captured").GetBoolean()
            );
        }

        // Verifies the authenticity of Razorpay webhook requests.

        public bool VerifyWebhookSignature(string payload, string signature)
        {
            if (string.IsNullOrWhiteSpace(payload) ||
                string.IsNullOrWhiteSpace(signature) ||
                string.IsNullOrWhiteSpace(_settings.WebhookSecret))
            {
                return false;
            }

            try
            {
                // Generate HMAC-SHA256 from the exact raw request payload.
                using var hmac = new HMACSHA256(
                    Encoding.UTF8.GetBytes(_settings.WebhookSecret));

                var hash = hmac.ComputeHash(
                    Encoding.UTF8.GetBytes(payload));

                // Convert the expected hash to hexadecimal.
                var expectedSignature = Convert.ToHexString(hash);

                // Normalize the received hexadecimal signature.
                var receivedSignature = signature.Trim().ToUpperInvariant();

                // Compare signatures safely.
                return CryptographicOperations.FixedTimeEquals(
                    Convert.FromHexString(expectedSignature),
                    Convert.FromHexString(receivedSignature));
            }
            catch (FormatException)
            {
                return false;
            }
        }

    }
}