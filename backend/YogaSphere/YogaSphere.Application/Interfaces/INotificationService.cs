using System;
using System.Collections.Generic;
using System.Text;
using YogaSphere.Application.DTOs.Notification;

namespace YogaSphere.Application.Interfaces
{
    public interface INotificationService
    {
        //retrieves notifications for the specified user
        Task<IReadOnlyList<NotificationResponse>> GetMyNotificationsAsync(
        int userId);

        //marks a notification as read
        Task<bool> MarkAsReadAsync(int notificationId, int userId);

        //requests deletion of a notification
        Task<bool> DeleteAsync(int notificationId, int userId);
    }
}
