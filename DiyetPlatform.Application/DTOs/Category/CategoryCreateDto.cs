using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DiyetPlatform.Application.DTOs.Category
{
    public class CategoryCreateDto
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; }
        
        public IFormFile Image { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
} 