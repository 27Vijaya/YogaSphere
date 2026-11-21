using Microsoft.EntityFrameworkCore;
using YogaSphere.Application.Interfaces.Repositories;
using YogaSphere.Domain.Entities;
using YogaSphere.Infrastructure.Persistence;

namespace YogaSphere.Infrastructure.Repositories
{
    public class WebhookEventRepository : IWebhookEventRepository
    {
        private readonly YogaSphereDbContext _context;

        public WebhookEventRepository(YogaSphereDbContext context)
        {
            _context = context;
        }

        // Checks whether this Razorpay event has already been stored.
        public async Task<bool> ExistsAsync(string eventId)
        {
            return await _context.WebhookEvents
                .AnyAsync(webhookEvent => webhookEvent.EventId == eventId);
        }

        // Adds a processed webhook event to the database context.
        public async Task AddAsync(WebhookEvent webhookEvent)
        {
            await _context.WebhookEvents.AddAsync(webhookEvent);
        }

        // Persists the event record to SQL Server.
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}