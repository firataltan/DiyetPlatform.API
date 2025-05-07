using AutoMapper;
using DiyetPlatform.API.Data.UnitOfWork;
using DiyetPlatform.API.Helpers;
using DiyetPlatform.API.Models;
using DiyetPlatform.API.Models.DTOs;
using DiyetPlatform.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiyetPlatform.API.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NotificationService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedList<NotificationDto>> GetUserNotificationsAsync(int userId, NotificationParams notificationParams)
        {
            var query = _unitOfWork.NotificationRepository.GetQuery()
                .Where(n => n.UserId == userId);

            if (notificationParams.OnlyUnread)
                query = query.Where(n => !n.IsRead);

            IOrderedQueryable<Notification> orderedQuery;
            if (notificationParams.OrderBy == "oldest")
                orderedQuery = query.OrderBy(n => n.CreatedAt);
            else
                orderedQuery = query.OrderByDescending(n => n.CreatedAt);

            var notifications = await PagedList<Notification>.CreateAsync(
                orderedQuery.Include(n => n.FromUser).ThenInclude(u => u.Profile),
                notificationParams.PageNumber,
                notificationParams.PageSize);

            var notificationDtos = _mapper.Map<IEnumerable<NotificationDto>>(notifications);
            
            return new PagedList<NotificationDto>(
                notificationDtos.ToList(),
                notifications.TotalCount,
                notifications.CurrentPage,
                notifications.PageSize);
        }

        public async Task<int> GetUnreadNotificationsCountAsync(int userId)
        {
            return await _unitOfWork.NotificationRepository.GetQuery()
                .Where(n => n.UserId == userId && !n.IsRead)
                .CountAsync();
        }

        public async Task<ServiceResponse<object>> MarkNotificationAsReadAsync(int userId, int notificationId)
        {
            var response = new ServiceResponse<object>();

            var notification = await _unitOfWork.NotificationRepository.GetQuery()
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification == null)
            {
                response.Success = false;
                response.Message = "Bildirim bulunamadı.";
                return response;
            }

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;

            _unitOfWork.NotificationRepository.Update(notification);
            await _unitOfWork.Complete();

            response.Message = "Bildirim okundu olarak işaretlendi.";
            return response;
        }

        public async Task<ServiceResponse<object>> MarkAllNotificationsAsReadAsync(int userId)
        {
            var response = new ServiceResponse<object>();

            var notifications = await _unitOfWork.NotificationRepository.GetQuery()
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            if (!notifications.Any())
            {
                response.Message = "Okunmamış bildirim bulunmamaktadır.";
                return response;
            }

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
            }

            _unitOfWork.NotificationRepository.UpdateRange(notifications);
            await _unitOfWork.Complete();

            response.Message = "Tüm bildirimler okundu olarak işaretlendi.";
            return response;
        }

        public async Task CreateNotificationAsync(int userId, int fromUserId, string type, int? postId = null, int? recipeId = null, int? commentId = null, string? message = null)
        {
            var notification = new Notification
            {
                UserId = userId,
                SenderId = fromUserId,
                Type = type,
                PostId = postId,
                RecipeId = recipeId,
                CommentId = commentId,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.NotificationRepository.AddAsync(notification);
            await _unitOfWork.Complete();
        }

        public async Task<ServiceResponse<object>> DeleteNotificationAsync(int userId, int notificationId)
        {
            var response = new ServiceResponse<object>();

            var notification = await _unitOfWork.NotificationRepository.GetNotificationByIdAsync(notificationId);

            if (notification == null)
            {
                response.Success = false;
                response.Message = "Bildirim bulunamadı.";
                return response;
            }

            if (notification.UserId != userId)
            {
                response.Success = false;
                response.Message = "Bu bildirimi silme yetkiniz yok.";
                return response;
            }

            _unitOfWork.NotificationRepository.Delete(notification);
            await _unitOfWork.Complete();

            response.Message = "Bildirim başarıyla silindi.";
            return response;
        }
    }
}