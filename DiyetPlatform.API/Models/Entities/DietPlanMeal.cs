using System;

namespace DiyetPlatform.API.Models.Entities
{
    public class DietPlanMeal
    {
        public int Id { get; set; }
        public int DietPlanId { get; set; }
        public string Title { get; set; } // Öğün adı (Kahvaltı, Öğle Yemeği, Akşam Yemeği vb.)
        public TimeSpan MealTime { get; set; } // Öğün saati
        public string FoodItems { get; set; } // JSON formatında yiyecek listesi
        public string NutritionInfo { get; set; } // JSON formatında besin değerleri
        
        // İlişkiler
        public DietPlan DietPlan { get; set; }
    }
} 