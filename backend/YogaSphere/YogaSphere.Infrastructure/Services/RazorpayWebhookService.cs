using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using YogaSphere.Application.Interfaces;
using YogaSphere.Application.Interfaces.Repositories;
using YogaSphere.Domain.Entities;
using YogaSphere.Domain.Enums;
using YogaSphere.Infrastructure.Persistence;

namespace YogaSphere.Infrastructure.Services
{
    public class RazorpayWebhookService : IRazorpayWebhookService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IWebhookEventRepository _webhookEventRepository;
        private readonly YogaSphereDbContext _context;

        public RazorpayWebhookService(IPaymentRepository paymentRepository,IWebhookEventRepository webhookEventRepository,YogaSphereDbContext context)
        {
            _paymentRepository = paymentRepository;
            _webhookEventRepository = webhookEventRepository;
            _context = context;
        }

        public async Task ProcessWebhookAsync(string payload)
        {
            using var document = JsonDocument.Parse(payload);

            var root = document.RootElement;

            var eventName = root.GetProperty("event").GetString();

            // Read Razorpay's unique event ID.
            var eventId = root.GetProperty("id").GetString();

            if (string.IsNullOrWhiteSpace(eventId))
            {
                throw new InvalidOperationException("Invalid Razorpay webhook event ID.");
            }

            // Ignore events that have already been processed.
            if (await _webhookEventRepository.ExistsAsync(eventId))
            {
                return;
            }

            // Read the Razorpay payment entity from the webhook payload.
            var paymentEntity = root
                .GetProperty("payload")
                .GetProperty("payment")
                .GetProperty("entity");

            var orderId = paymentEntity.GetProperty("order_id").GetString();

            var paymentId = paymentEntity.GetProperty("id").GetString();
            var razorpayAmount = paymentEntity.GetProperty("amount").GetInt64();
            var razorpayCurrency = paymentEntity.GetProperty("currency").GetString();

            if (string.IsNullOrWhiteSpace(orderId) ||
                string.IsNullOrWhiteSpace(paymentId))
            {
                throw new InvalidOperationException(
                    "Invalid Razorpay webhook payload.");
            }

            // Ensure the webhook contains a valid Razorpay payment ID.
            if (!paymentId.StartsWith("pay_", StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Invalid Razorpay payment ID.");
            }

            var payment = await _paymentRepository.GetByGatewayOrderIdAsync(orderId);

            if (payment == null)
                return;

            if (!string.Equals(payment.GatewayOrderId,orderId,StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "Razorpay order ID does not match the local payment.");
            }

            // Razorpay reports amounts in paise, while our database stores rupees.
            var expectedAmount = (long)Math.Round(payment.Amount * 100m, MidpointRounding.AwayFromZero);

            // Reject events that don't match the expected payment amount or currency.
            if (razorpayAmount != expectedAmount ||
                !string.Equals(razorpayCurrency, "INR", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Webhook payment amount or currency does not match.");
            }

            // Ignore events for payments that are no longer pending.
            if (payment.PaymentStatus != PaymentStatus.Pending)
                return;

            if(eventName != "payment.captured" && eventName != "payment.failed")
            {
                return;
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            switch (eventName)
            {
                case "payment.captured":

                    if (payment.Booking == null)
                    {
                        throw new InvalidOperationException(
                            "Booking not found for this payment.");
                    }

                    payment.PaymentStatus = PaymentStatus.Successful;
                    payment.TransactionId = paymentId;
                    payment.PaidAt = DateTime.UtcNow;

                    // Confirm the booking only if it has not been cancelled.
                    // A cancelled booking must remain cancelled for manual review.
                    if (payment.Booking.Status != BookingStatus.Cancelled)
                    {
                        payment.Booking.Status = BookingStatus.Confirmed;
                    }
                    break;

                case "payment.failed":
                    payment.PaymentStatus = PaymentStatus.Failed;
                    payment.TransactionId = paymentId;
                    break;

                default:
                    // Ignore events that YogaSphere does not process.
                    return;
            }

            try
            {
                // Store the processed event and save payment changes together.
                await _webhookEventRepository.AddAsync(new WebhookEvent
                {
                    EventId = eventId,
                    EventType = eventName!,
                    ProcessedAt = DateTime.UtcNow
                });

                await _paymentRepository.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (DbUpdateException ex) when (
                ex.InnerException is SqlException sqlException &&
                (sqlException.Number == 2601 || sqlException.Number == 2627))
            {
                // Another request already stored this event.
                await transaction.RollbackAsync();
            }
        }
    }
}