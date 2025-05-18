using DiyetPlatform.Core.Common;
using DiyetPlatform.Application.Common.Parameters;
using DiyetPlatform.Application.DTOs.Notification;

namespace DiyetPlatform.Application.Interfaces
{
    public interface INotificationService
    {
        Task<PagedList<NotificationDto>> GetUserNotificationsAsync(int userId, NotificationParams notificationParams);
        Task<int> GetUnreadNotificationsCountAsync(int userId);
        Task<ServiceResponse<object>> MarkNotificationAsReadAsync(int userId, int notificationId);
        Task<ServiceResponse<object>> MarkAllNotificationsAsReadAsync(int userId);
        Task<ServiceResponse<object>> CreateNotificationAsync(
            int userId, 
            int actorId, 
            string message, 
            string type, 
            int? postId = null, 
            int? recipeId = null, 
            int? commentId = null);
        Task<ServiceResponse<object>> DeleteNotificationAsync(int userId, int notificationId);
    }
}
