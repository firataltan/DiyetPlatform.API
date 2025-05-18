using AutoMapper;
using DiyetPlatform.Core.Entities;
using DiyetPlatform.Core.Common;
using DiyetPlatform.Application.DTOs.Notification;
using DiyetPlatform.Application.Interfaces;
using DiyetPlatform.Application.Common.Parameters;
//using DiyetPlatform.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiyetPlatform.Core.Interfaces;

namespace DiyetPlatform.Application.Services
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
            var query = _unitOfWork.Notifications.GetNotificationsQuery()
                .Where(n => n.UserId == userId);

            if (notificationParams.OnlyUnread)
                query = query.Where(n => !n.IsRead);

            // Apply sorting
            query = notificationParams.OrderBy == "oldest"
                ? query.OrderBy(n => n.CreatedAt)
                : query.OrderByDescending(n => n.CreatedAt);

            var totalCount = await query.CountAsync();
            var notifications = await query
                .Skip((notificationParams.PageNumber - 1) * notificationParams.PageSize)
                .Take(notificationParams.PageSize)
                .ToListAsync();

            var notificationDtos = _mapper.Map<List<NotificationDto>>(notifications);
            
            return new PagedList<NotificationDto>(
                notificationDtos,
                totalCount,
                notificationParams.PageNumber,
                notificationParams.PageSize);
        }

        public async Task<int> GetUnreadNotificationsCountAsync(int userId)
        {
            return await _unitOfWork.Notifications.GetUnreadNotificationCountAsync(userId);
        }

        public async Task<ServiceResponse<object>> MarkNotificationAsReadAsync(int userId, int notificationId)
        {
            var response = new ServiceResponse<object>();

            try
            {
                var notification = await _unitOfWork.Notifications.GetNotificationByIdAsync(notificationId);

                if (notification == null)
                {
                    response.Success = false;
                    response.Message = "Bildirim bulunamadı.";
                    return response;
                }

                if (notification.UserId != userId)
                {
                    response.Success = false;
                    response.Message = "Bu bildirimi işaretleme yetkiniz yok.";
                    return response;
                }

                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;

                _unitOfWork.Notifications.Update(notification);
                await _unitOfWork.Complete();

                response.Message = "Bildirim okundu olarak işaretlendi.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Bildirim işaretlenirken bir hata oluştu: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<object>> MarkAllNotificationsAsReadAsync(int userId)
        {
            var response = new ServiceResponse<object>();

            try
            {
                var notifications = await _unitOfWork.Notifications.GetNotificationsQuery()
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

                _unitOfWork.Notifications.UpdateRange(notifications);
                await _unitOfWork.Complete();

                response.Message = "Tüm bildirimler okundu olarak işaretlendi.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Bildirimler işaretlenirken bir hata oluştu: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<object>> CreateNotificationAsync(
            int userId, 
            int actorId, 
            string message, 
            string type, 
            int? postId = null, 
            int? recipeId = null, 
            int? commentId = null)
        {
            var response = new ServiceResponse<object>();

            try
            {
                var notification = new Notification
                {
                    UserId = userId,
                    ActorId = actorId,
                    Type = type,
                    Message = message,
                    PostId = postId,
                    RecipeId = recipeId,
                    CommentId = commentId,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                _unitOfWork.Notifications.Add(notification);
                await _unitOfWork.Complete();

                response.Message = "Bildirim başarıyla oluşturuldu.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Bildirim oluşturulurken bir hata oluştu: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<object>> DeleteNotificationAsync(int userId, int notificationId)
        {
            var response = new ServiceResponse<object>();

            try
            {
                var notification = await _unitOfWork.Notifications.GetNotificationByIdAsync(notificationId);

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

                _unitOfWork.Notifications.Delete(notification);
                await _unitOfWork.Complete();

                response.Message = "Bildirim başarıyla silindi.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Bildirim silinirken bir hata oluştu: {ex.Message}";
            }

            return response;
        }
    }
}