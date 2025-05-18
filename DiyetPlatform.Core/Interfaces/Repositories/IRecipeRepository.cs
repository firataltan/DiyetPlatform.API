using DiyetPlatform.Core.Entities;
using DiyetPlatform.Core.Common;

namespace DiyetPlatform.Core.Interfaces.Repositories
{
    public interface IRecipeRepository : IGenericRepository<Recipe>
    {
        Task<Recipe> GetRecipeByIdAsync(int id);
        Task<PagedList<Recipe>> GetRecipesAsync(RecipeParams recipeParams);
        Task<PagedList<Recipe>> GetUserRecipesAsync(int userId, RecipeParams recipeParams);
        Task<PagedList<Recipe>> GetRecipesByCategoryAsync(int categoryId, RecipeParams recipeParams);
        Task<PagedList<Recipe>> SearchRecipesAsync(string searchTerm, RecipeParams recipeParams);
        Task<PagedList<Recipe>> GetSavedRecipesAsync(int userId, RecipeParams recipeParams);
    }
} 