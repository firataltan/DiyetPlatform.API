using Microsoft.EntityFrameworkCore;
using DiyetPlatform.API.Helpers;
using DiyetPlatform.API.Models.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace DiyetPlatform.API.Data.Repositories
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
                .Include(n => n.FromUser)
                    .ThenInclude(u => u.Profile)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<PagedList<Notification>> GetUserNotificationsAsync(int userId, NotificationParams notificationParams)
        {
            var query = _context.Notifications
                .Include(n => n.FromUser)
                .ThenInclude(u => u.Profile)
                .Where(n => n.UserId == userId)
                .AsQueryable();

            query = notificationParams.OrderBy switch
            {
                "oldest" => query.OrderBy(n => n.CreatedAt),
                _ => query.OrderByDescending(n => n.CreatedAt)
            };

            return await PagedList<Notification>.CreateAsync(query, notificationParams.PageNumber, notificationParams.PageSize);
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

        public IQueryable<Notification> GetQuery()
        {
            return _context.Notifications.AsQueryable();
        }

        public async Task<List<Notification>> GetUserNotificationsAsync(int userId)
        {
            return await _context.Notifications
                .Include(n => n.FromUser)
                    .ThenInclude(u => u.Profile)
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public void UpdateRange(IEnumerable<Notification> notifications)
        {
            _context.Notifications.UpdateRange(notifications);
        }
    }
}
