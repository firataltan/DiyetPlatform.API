using Microsoft.AspNetCore.Http;

namespace DiyetPlatform.API.Models.DTOs.Recipe
{
    public class RecipeUpdateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Ingredients { get; set; }
        public string Instructions { get; set; }
        public IFormFile Image { get; set; }
        public int PrepTimeMinutes { get; set; }
        public int CookTimeMinutes { get; set; }
        public int Calories { get; set; }
        public Dictionary<string, string> NutritionInfo { get; set; }
        public List<int> CategoryIds { get; set; }
    }
}