namespace DiyetPlatform.API.Models.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SenderId { get; set; }
        public string Type { get; set; } // Like, Comment, Follow vb.
        public int? PostId { get; set; }
        public int? RecipeId { get; set; }
        public int? CommentId { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // İlişkiler
        public User User { get; set; }
        public User FromUser { get; set; }
        public Post Post { get; set; }
        public Recipe Recipe { get; set; }
        public Comment Comment { get; set; }
    }
}