using DiyetPlatform.API.Helpers;
using DiyetPlatform.API.Models.DTOs.Post;
using DiyetPlatform.API.Models.DTOs.Recipe;
using DiyetPlatform.API.Models;

namespace DiyetPlatform.API.Services
{
    public interface IRecipeService
    {
        Task<PagedList<RecipeResponseDto>> GetRecipesAsync(RecipeParams recipeParams);
        Task<RecipeResponseDto> GetRecipeByIdAsync(int id);
        Task<PagedList<RecipeResponseDto>> GetUserRecipesAsync(int userId, RecipeParams recipeParams);
        Task<PagedList<RecipeResponseDto>> GetRecipesByCategoryAsync(int categoryId, RecipeParams recipeParams);
        Task<RecipeResponseDto> CreateRecipeAsync(int userId, RecipeCreateDto recipeDto);
        Task<ServiceResponse<RecipeResponseDto>> UpdateRecipeAsync(int userId, int recipeId, RecipeUpdateDto recipeDto);
        Task<ServiceResponse<object>> DeleteRecipeAsync(int userId, int recipeId);
        Task<ServiceResponse<object>> LikeRecipeAsync(int userId, int recipeId);
        Task<ServiceResponse<object>> UnlikeRecipeAsync(int userId, int recipeId);
        Task<ServiceResponse<CommentDto>> AddCommentAsync(int userId, int recipeId, CommentCreateDto commentDto);
        Task<PagedList<RecipeResponseDto>> SearchRecipesAsync(string searchTerm, RecipeParams recipeParams);
    }
}