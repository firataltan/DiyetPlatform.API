﻿using System;
using System.Collections.Generic;

namespace DiyetPlatform.Application.DTOs.DietPlan
{
    public class DietPlanResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        
        public int UserId { get; set; }
        public string Username { get; set; }
        public string UserProfileImage { get; set; }
        
        public int? DietitianId { get; set; }
        public string DietitianName { get; set; }
        public string DietitianProfileImage { get; set; }
        
        public string DietType { get; set; }
        public int DurationDays { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        public List<DietPlanMealDto> Meals { get; set; }
    }

    public class DietPlanMealResponseDto
    {
        public int Id { get; set; }
        public string MealType { get; set; }
        public string Description { get; set; }
        public int Calories { get; set; }
        public Dictionary<string, string> NutritionInfo { get; set; }
    }
}