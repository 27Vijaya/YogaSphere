using YogaSphere.Domain.Entities;

namespace YogaSphere.Application.Interfaces.Repositories
{
    public interface IWebhookEventRepository
    {
        // Checks whether a webhook event has already been processed.
        Task<bool> ExistsAsync(string eventId);

        // Stores a successfully processed webhook event.
        Task AddAsync(WebhookEvent webhookEvent);

        // Saves changes to the database.
        Task SaveChangesAsync();
    }
}