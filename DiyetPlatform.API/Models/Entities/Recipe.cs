using System.Xml.Linq;

namespace DiyetPlatform.API.Models.Entities
{
    public class Recipe
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Ingredients { get; set; } // JSON formatında malzemeler
        public string Instructions { get; set; }
        public string ImageUrl { get; set; }
        public int PrepTimeMinutes { get; set; }
        public int CookTimeMinutes { get; set; }
        public int Calories { get; set; }
        public string NutritionInfo { get; set; } // JSON formatında besin değerleri
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // İlişkiler
        public User User { get; set; }
        public ICollection<RecipeCategory> Categories { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Like> Likes { get; set; }
    }

    public class RecipeCategory
    {
        public int RecipeId { get; set; }
        public int CategoryId { get; set; }

        // İlişkiler
        public Recipe Recipe { get; set; }
        public Category Category { get; set; }
    }
}
