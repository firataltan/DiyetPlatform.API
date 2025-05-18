namespace DiyetPlatform.Core.Entities;

public class Like
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // İlişkiler
    public int UserId { get; set; }
    public User User { get; set; }
    public int? PostId { get; set; }
    public Post Post { get; set; }
    public int? RecipeId { get; set; }
    public Recipe Recipe { get; set; }
}