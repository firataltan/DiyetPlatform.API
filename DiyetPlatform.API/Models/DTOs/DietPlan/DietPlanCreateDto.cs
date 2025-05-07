using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DiyetPlatform.API.Models.DTOs.DietPlan
{
    public class DietPlanCreateDto
    {
        [Required]
        public string Title { get; set; }
        
        public string Description { get; set; }
        
        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public List<DietPlanMealDto> Meals { get; set; }
    }
}
