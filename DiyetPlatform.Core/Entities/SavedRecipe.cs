using System;

namespace DiyetPlatform.Core.Entities
{
    public class SavedRecipe
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RecipeId { get; set; }
        public DateTime SavedAt { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Recipe Recipe { get; set; }
    }
} 