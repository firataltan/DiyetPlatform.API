using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DiyetPlatform.Application.Interfaces;
using DiyetPlatform.Application.Common.Parameters;

namespace DiyetPlatform.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] NotificationParams notificationParams)
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var notifications = await _notificationService.GetUserNotificationsAsync(userId, notificationParams);
            return Ok(notifications);
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadNotificationsCount()
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var count = await _notificationService.GetUnreadNotificationsCountAsync(userId);
            return Ok(new { unreadCount = count });
        }

        [HttpPost("{id}/mark-as-read")]
        public async Task<IActionResult> MarkNotificationAsRead(int id)
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var result = await _notificationService.MarkNotificationAsReadAsync(userId, id);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("mark-all-as-read")]
        public async Task<IActionResult> MarkAllNotificationsAsRead()
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var result = await _notificationService.MarkAllNotificationsAsReadAsync(userId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var result = await _notificationService.DeleteNotificationAsync(userId, id);
            
            if (!result.Success)
                return BadRequest(result.Message);
            
            return Ok(result);
        }
    }
}