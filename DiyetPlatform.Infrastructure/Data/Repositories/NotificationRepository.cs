using Microsoft.EntityFrameworkCore;
using DiyetPlatform.Core.Entities;
using DiyetPlatform.Core.Interfaces.Repositories;
using DiyetPlatform.Core.Common;
using DiyetPlatform.Infrastructure.Data.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiyetPlatform.Infrastructure.Data.Repositories
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Notification> GetNotificationByIdAsync(int id)
        {
            return await _context.Notifications
                .Include(n => n.User)
                .Include(n => n.Actor)
                    .ThenInclude(u => u.Profile)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public IQueryable<Notification> GetNotificationsQuery()
        {
            return _context.Notifications.AsQueryable();
        }

        public async Task<int> GetUnreadNotificationCountAsync(int userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .CountAsync();
        }

        public async Task<bool> IsNotificationExistsAsync(int id)
        {
            return await _context.Notifications.AnyAsync(n => n.Id == id);
        }

        public async Task<bool> IsUserOwnerOfNotificationAsync(int userId, int notificationId)
        {
            return await _context.Notifications.AnyAsync(n => n.Id == notificationId && n.UserId == userId);
        }

        public void UpdateRange(IEnumerable<Notification> notifications)
        {
            _context.Notifications.UpdateRange(notifications);
        }

        public new void Delete(Notification notification)
        {
            _context.Notifications.Remove(notification);
        }
    }
}
