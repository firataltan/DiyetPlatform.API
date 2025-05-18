using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace DiyetPlatform.Application.DTOs.Recipe
{
    public class RecipeUpdateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }
        public List<string> Ingredients { get; set; }
        public List<string> Instructions { get; set; }
        public Dictionary<string, string> NutritionInfo { get; set; }
        public string DifficultyLevel { get; set; }
        public int CookingTime { get; set; }
        public string CuisineType { get; set; }
        public List<int> CategoryIds { get; set; }
    }
} 