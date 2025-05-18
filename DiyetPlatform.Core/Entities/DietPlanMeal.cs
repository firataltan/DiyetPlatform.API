using System;

namespace DiyetPlatform.Core.Entities;

public class DietPlanMeal
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string FoodItems { get; set; }
    public string NutritionInfo { get; set; }
    public DateTime MealTime { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // İlişkiler
    public int DietPlanId { get; set; }
    public DietPlan DietPlan { get; set; }
} 