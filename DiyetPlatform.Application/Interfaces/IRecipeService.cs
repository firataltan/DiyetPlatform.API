using DiyetPlatform.Core.Common;
using DiyetPlatform.Application.Common.Parameters;
using DiyetPlatform.Application.DTOs.Recipe;
using DiyetPlatform.Application.DTOs.Post;

namespace DiyetPlatform.Application.Interfaces
{
    public interface IRecipeService
    {
        Task<PagedList<RecipeResponseDto>> GetRecipesAsync(DiyetPlatform.Core.Common.RecipeParams recipeParams);
        Task<RecipeResponseDto> GetRecipeByIdAsync(int id);
        Task<PagedList<RecipeResponseDto>> GetUserRecipesAsync(int userId, DiyetPlatform.Core.Common.RecipeParams recipeParams);
        Task<PagedList<RecipeResponseDto>> GetRecipesByCategoryAsync(int categoryId, DiyetPlatform.Core.Common.RecipeParams recipeParams);
        Task<RecipeResponseDto> CreateRecipeAsync(int userId, RecipeCreateDto recipeDto);
        Task<ServiceResponse<RecipeResponseDto>> UpdateRecipeAsync(int userId, int recipeId, RecipeUpdateDto recipeDto);
        Task<ServiceResponse<object>> DeleteRecipeAsync(int userId, int recipeId);
        Task<ServiceResponse<object>> LikeRecipeAsync(int userId, int recipeId);
        Task<ServiceResponse<object>> UnlikeRecipeAsync(int userId, int recipeId);
        Task<ServiceResponse<CommentDto>> AddCommentAsync(int userId, int recipeId, CommentCreateDto commentDto);
        Task<PagedList<RecipeResponseDto>> SearchRecipesAsync(string searchTerm, DiyetPlatform.Core.Common.RecipeParams recipeParams);
        Task<ServiceResponse<object>> SaveRecipeAsync(int userId, int recipeId);
        Task<ServiceResponse<object>> UnsaveRecipeAsync(int userId, int recipeId);
        Task<PagedList<RecipeResponseDto>> GetSavedRecipesAsync(int userId, DiyetPlatform.Core.Common.RecipeParams recipeParams);
    }
}