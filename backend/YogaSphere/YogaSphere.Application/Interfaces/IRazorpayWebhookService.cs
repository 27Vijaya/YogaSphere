namespace YogaSphere.Application.Interfaces
{
    public interface IRazorpayWebhookService
    {
        Task ProcessWebhookAsync(string payload);
    }
}