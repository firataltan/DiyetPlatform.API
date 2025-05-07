using DiyetPlatform.API.Helpers;
using DiyetPlatform.API.Models;
using DiyetPlatform.API.Models.DTOs;
using DiyetPlatform.API.Models.Entities;

namespace DiyetPlatform.API.Services
{
    public interface INotificationService
    {
        Task<PagedList<NotificationDto>> GetUserNotificationsAsync(int userId, NotificationParams notificationParams);
        Task<int> GetUnreadNotificationsCountAsync(int userId);
        Task<ServiceResponse<object>> MarkNotificationAsReadAsync(int userId, int notificationId);
        Task<ServiceResponse<object>> MarkAllNotificationsAsReadAsync(int userId);
        Task CreateNotificationAsync(int userId, int fromUserId, string type, int? postId = null, int? recipeId = null, int? commentId = null, string? message = null);
        Task<ServiceResponse<object>> DeleteNotificationAsync(int userId, int notificationId);
    }
}
