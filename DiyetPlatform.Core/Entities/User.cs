namespace DiyetPlatform.Core.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // İlişkiler
    public Profile Profile { get; set; }
    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
    public ICollection<DietPlan> DietPlans { get; set; } = new List<DietPlan>();
    public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
    public ICollection<Follow> Followers { get; set; } = new List<Follow>();
    public ICollection<Follow> Following { get; set; } = new List<Follow>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<SavedRecipe> SavedRecipes { get; set; } = new List<SavedRecipe>();
}