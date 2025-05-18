namespace DiyetPlatform.Core.Entities;

public class Notification
{
    public int Id { get; set; }
    public string Type { get; set; }
    public string Message { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReadAt { get; set; }

    // İlişkiler
    public int UserId { get; set; }
    public User User { get; set; }
    public int? ActorId { get; set; }
    public User Actor { get; set; }
    public int? PostId { get; set; }
    public Post Post { get; set; }
    public int? RecipeId { get; set; }
    public Recipe Recipe { get; set; }
    public int? CommentId { get; set; }
    public Comment Comment { get; set; }
}