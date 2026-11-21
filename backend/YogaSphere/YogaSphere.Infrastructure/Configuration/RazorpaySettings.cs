namespace YogaSphere.Infrastructure.Configuration
{
    public class RazorpaySettings
    {
        public string KeyId { get; set; } = string.Empty;
        public string KeySecret { get; set; } = string.Empty;

        // Used to verify Razorpay webhook signatures
        public string WebhookSecret { get; set; } = string.Empty;
    }
}