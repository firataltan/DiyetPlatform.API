using DiyetPlatform.API.Helpers;
using DiyetPlatform.API.Models.Entities;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DiyetPlatform.API.Data.Repositories
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        IQueryable<Notification> GetQuery();
        Task<Notification> GetNotificationByIdAsync(int id);
        Task<PagedList<Notification>> GetUserNotificationsAsync(int userId, NotificationParams notificationParams);
        Task<int> GetUnreadNotificationCountAsync(int userId);
        Task<bool> IsNotificationExistsAsync(int id);
        Task<bool> IsUserOwnerOfNotificationAsync(int userId, int notificationId);
        void UpdateRange(IEnumerable<Notification> notifications);
    }
}