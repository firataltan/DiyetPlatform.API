using System;
using System.Collections.Generic;
using DiyetPlatform.Application.DTOs.Post;
using DiyetPlatform.Application.DTOs.Category;

namespace DiyetPlatform.Application.DTOs.Recipe
{
    public class RecipeResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string UserProfileImage { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public List<string> Ingredients { get; set; }
        public List<string> Instructions { get; set; }
        public Dictionary<string, string> NutritionInfo { get; set; }
        public string DifficultyLevel { get; set; }
        public int CookingTime { get; set; }
        public string CuisineType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public bool IsLiked { get; set; }
        public bool IsSaved { get; set; }
        public List<CategoryResponseDto> Categories { get; set; }
        public List<CommentDto> Comments { get; set; }
    }
}
