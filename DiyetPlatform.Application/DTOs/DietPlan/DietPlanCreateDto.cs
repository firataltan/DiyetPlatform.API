using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DiyetPlatform.Application.DTOs.DietPlan
{
    public class DietPlanCreateDto
    {
        [Required]
        public string Title { get; set; }
        
        public string Description { get; set; }
        
        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public string DietType { get; set; }
        
        public int DurationDays { get; set; }
        
        public List<DietPlanMealCreateDto> Meals { get; set; }
    }

    public class DietPlanMealCreateDto
    {
        public string Title { get; set; }
        public string Type { get; set; }  // Breakfast, Lunch, Dinner, Snack
        public int Day { get; set; }
        public List<FoodItemCreateDto> FoodItems { get; set; }
        public Dictionary<string, string> NutritionInfo { get; set; }
    }

    public class FoodItemCreateDto
    {
        public string Name { get; set; }
        public string Amount { get; set; }
        public string Unit { get; set; }
        public int? RecipeId { get; set; }
    }
}
