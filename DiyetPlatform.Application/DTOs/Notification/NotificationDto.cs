using System;

namespace DiyetPlatform.Application.DTOs.Notification
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? ActorId { get; set; }
        public string ActorName { get; set; }
        public string ActorProfilePicture { get; set; }
        public string Type { get; set; }
        public int? PostId { get; set; }
        public int? RecipeId { get; set; }
        public int? CommentId { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
    }
} 