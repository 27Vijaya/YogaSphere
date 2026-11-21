namespace YogaSphere.Application.Interfaces
{
    public interface IRazorpayService
    {
        // Creates an order with the Razorpay payment gateway.
        Task<(string OrderId, string KeyId)> CreateOrderAsync(decimal amount,string currency);

        // Verifies the payment signature returned by Razorpay.
        bool VerifyPaymentSignature(string orderId,string paymentId,string signature);

        Task<(string OrderId, long Amount, string Currency, string Status, bool Captured)>
            GetPaymentDetailsAsync(string paymentId);

        // Verifies that a webhook request genuinely came from Razorpay.
        bool VerifyWebhookSignature(string payload, string signature);
    }
}