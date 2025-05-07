using DiyetPlatform.API.Helpers;
using DiyetPlatform.API.Models.DTOs;
using DiyetPlatform.API.Models.DTOs.User;
using DiyetPlatform.API.Models.DTOs.Post;
using DiyetPlatform.API.Models.DTOs.Recipe;
using DiyetPlatform.API.Models.DTOs.DietPlan;

namespace DiyetPlatform.API.Services
{
    public interface ISearchService
    {
        Task<SearchResultDto> SearchAsync(string query, Helpers.SearchParams searchParams);
        Task<PagedList<UserDto>> SearchUsersAsync(string query, UserParams userParams);
        Task<PagedList<PostResponseDto>> SearchPostsAsync(string query, PostParams postParams);
        Task<PagedList<RecipeResponseDto>> SearchRecipesAsync(string query, RecipeParams recipeParams);
        Task<PagedList<DietPlanResponseDto>> SearchDietPlansAsync(string query, DietPlanParams dietPlanParams);
    }
}
