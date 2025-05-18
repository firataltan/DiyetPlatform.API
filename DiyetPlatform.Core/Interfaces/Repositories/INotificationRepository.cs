using DiyetPlatform.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace DiyetPlatform.Core.Interfaces.Repositories
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<Notification> GetNotificationByIdAsync(int id);
        IQueryable<Notification> GetNotificationsQuery();
        Task<int> GetUnreadNotificationCountAsync(int userId);
        Task<bool> IsNotificationExistsAsync(int id);
        Task<bool> IsUserOwnerOfNotificationAsync(int userId, int notificationId);
        new void Add(Notification notification);
        new void Update(Notification notification);
        new void Delete(Notification notification);
        void UpdateRange(IEnumerable<Notification> notifications);
    }
}