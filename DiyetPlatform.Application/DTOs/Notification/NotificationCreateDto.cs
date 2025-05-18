using System.ComponentModel.DataAnnotations;

namespace DiyetPlatform.Application.DTOs.Notification
{
    public class NotificationCreateDto
    {
        [Required]
        public string Type { get; set; }
        
        [Required]
        public string Message { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        public int? ActorId { get; set; }
        
        public int? PostId { get; set; }
        
        public int? RecipeId { get; set; }
        
        public int? CommentId { get; set; }
    }
} 