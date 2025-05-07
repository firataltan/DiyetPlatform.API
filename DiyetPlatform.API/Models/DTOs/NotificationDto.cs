namespace DiyetPlatform.API.Models.DTOs
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? FromUserId { get; set; }
        public string FromUsername { get; set; }
        public string FromUserProfileImage { get; set; }
        public string Type { get; set; }
        public int? PostId { get; set; }
        public int? RecipeId { get; set; }
        public int? CommentId { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}