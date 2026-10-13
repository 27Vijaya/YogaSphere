using System;
using System.Collections.Generic;
using System.Text;
using YogaSphere.Domain.Entities;

namespace YogaSphere.Application.Interfaces.Repositories
{
    public interface INotificationRepository
    {
        Task<IReadOnlyList<Notification>> GetByUserIdAsync(int userId);

        Task<Notification?> GetByIdAsync(int notificationId);

        Task AddAsync(Notification notification);

        Task DeleteAsync(Notification notification);

        Task SaveChangesAsync();
    }
}
