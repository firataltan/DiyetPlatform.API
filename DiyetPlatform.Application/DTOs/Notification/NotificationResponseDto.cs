using System;

namespace DiyetPlatform.Application.DTOs.Notification
{
    public class NotificationResponseDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public int UserId { get; set; }
        public int? ActorId { get; set; }
        public string ActorName { get; set; }
        public string ActorProfileImage { get; set; }
        public int? PostId { get; set; }
        public int? RecipeId { get; set; }
        public int? CommentId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
} 