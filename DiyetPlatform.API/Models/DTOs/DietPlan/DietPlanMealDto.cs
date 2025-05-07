using System;
using System.Collections.Generic;

namespace DiyetPlatform.API.Models.DTOs.DietPlan
{
    public class DietPlanMealDto
    {
        public int Id { get; set; }
        public string Title { get; set; } // Öğün adı (Kahvaltı, Öğle Yemeği, Akşam Yemeği vb.)
        public TimeSpan MealTime { get; set; } // Öğün saati
        public List<string> FoodItems { get; set; } // Yiyecek listesi
        public Dictionary<string, string> NutritionInfo { get; set; } // Besin değerleri
    }
} 