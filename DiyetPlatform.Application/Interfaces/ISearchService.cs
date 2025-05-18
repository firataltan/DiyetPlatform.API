using DiyetPlatform.Core.Common;
using DiyetPlatform.Application.Common.Parameters;
using DiyetPlatform.Application.DTOs.User;
using DiyetPlatform.Application.DTOs.Post;
using DiyetPlatform.Application.DTOs.Recipe;
using DiyetPlatform.Application.DTOs.DietPlan;
using DiyetPlatform.Application.DTOs.Search;

namespace DiyetPlatform.Application.Interfaces
{
    public interface ISearchService
    {
        Task<SearchResultDto> SearchAllAsync(string searchTerm);
        Task<SearchResultDto> SearchAsync(string query, SearchParams searchParams);
        Task<PagedList<UserDto>> SearchUsersAsync(string searchTerm, DiyetPlatform.Core.Common.UserParams userParams);
        Task<PagedList<PostResponseDto>> SearchPostsAsync(string searchTerm, DiyetPlatform.Core.Common.PostParams postParams);
        Task<PagedList<RecipeResponseDto>> SearchRecipesAsync(string searchTerm, DiyetPlatform.Core.Common.RecipeParams recipeParams);
        Task<PagedList<DietPlanResponseDto>> SearchDietPlansAsync(string searchTerm, DietPlanParams dietPlanParams);
    }
}
