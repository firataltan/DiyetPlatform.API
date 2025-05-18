using System.Xml.Linq;

namespace DiyetPlatform.Core.Entities;

public class Recipe
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public string Ingredients { get; set; }
    public string Instructions { get; set; }
    public string NutritionInfo { get; set; }
    public int PrepTime { get; set; }
    public int CookTime { get; set; }
    public int Servings { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // İlişkiler
    public int UserId { get; set; }
    public User User { get; set; }
    public ICollection<Like> Likes { get; set; } = new List<Like>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<RecipeCategory> Categories { get; set; } = new List<RecipeCategory>();
    public ICollection<SavedRecipe> SavedByUsers { get; set; } = new List<SavedRecipe>();
}

public class RecipeCategory
{
    public int RecipeId { get; set; }
    public int CategoryId { get; set; }

    // İlişkiler
    public Recipe Recipe { get; set; }
    public Category Category { get; set; }
}
