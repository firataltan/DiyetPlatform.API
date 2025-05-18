using System.Collections.Generic;

namespace DiyetPlatform.Application.DTOs.DietPlan
{
    public class DietPlanMealDto
    {
        public int Id { get; set; }
        public string Title { get; set; } // Öğün adı (Kahvaltı, Öğle Yemeği, Akşam Yemeği vb.)
        public string Type { get; set; }  // Breakfast, Lunch, Dinner, Snack
        public int Day { get; set; }
        public List<FoodItemDto> FoodItems { get; set; } // Yiyecek listesi
        public Dictionary<string, string> NutritionInfo { get; set; } // Besin değerleri
    }

    public class FoodItemDto
    {
        public string Name { get; set; }
        public string Amount { get; set; }
        public string Unit { get; set; }
        public int? RecipeId { get; set; }
        public string RecipeTitle { get; set; }
    }
} 