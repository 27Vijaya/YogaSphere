namespace YogaSphere.Domain.Entities
{
    public class WebhookEvent
    {
        public int Id { get; set; }

        public string EventId { get; set; } = string.Empty;

        // Event name, for example payment.captured.
        public string EventType { get; set; } = string.Empty;

        // When our application successfully processed the event.
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    }
}