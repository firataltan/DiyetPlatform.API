using DiyetPlatform.API.Helpers;
using DiyetPlatform.API.Models.DTOs.User;
using DiyetPlatform.API.Models.DTOs.Post;
using DiyetPlatform.API.Models.DTOs.Recipe;
using DiyetPlatform.API.Models.DTOs.DietPlan;

namespace DiyetPlatform.API.Models.DTOs
{
    public class SearchResultDto
    {
        public PagedList<UserDto> Users { get; set; }
        public PagedList<PostResponseDto> Posts { get; set; }
        public PagedList<RecipeResponseDto> Recipes { get; set; }
        public PagedList<DietPlanResponseDto> DietPlans { get; set; }
    }
}