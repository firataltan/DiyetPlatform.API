using DiyetPlatform.Core.Common;
using DiyetPlatform.Application.DTOs.User;
using DiyetPlatform.Application.DTOs.Post;
using DiyetPlatform.Application.DTOs.Recipe;
using DiyetPlatform.Application.DTOs.DietPlan;

namespace DiyetPlatform.Application.DTOs.Search
{
    public class SearchResultDto
    {
        public PagedList<UserDto> Users { get; set; }
        public PagedList<PostResponseDto> Posts { get; set; }
        public PagedList<RecipeResponseDto> Recipes { get; set; }
        public PagedList<DietPlanResponseDto> DietPlans { get; set; }
    }
} 