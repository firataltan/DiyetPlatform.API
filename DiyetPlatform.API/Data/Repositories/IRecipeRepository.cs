using DiyetPlatform.API.Helpers;
using DiyetPlatform.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiyetPlatform.API.Data.Repositories
{
    public interface IRecipeRepository : IGenericRepository<Recipe>
    {
        Task<Recipe> GetRecipeByIdAsync(int id);
        Task<PagedList<Recipe>> GetRecipesAsync(RecipeParams recipeParams);
        Task<PagedList<Recipe>> GetUserRecipesAsync(int userId, RecipeParams recipeParams);
        Task<PagedList<Recipe>> GetRecipesByCategoryAsync(int categoryId, RecipeParams recipeParams);
        Task<bool> IsRecipeExistsAsync(int id);
        Task<bool> IsUserOwnerOfRecipeAsync(int userId, int recipeId);
        Task<bool> IsRecipeLikedByUserAsync(int userId, int recipeId);
        Task<Like> GetRecipeLikeAsync(int userId, int recipeId);
        Task<Category> GetCategoryByIdAsync(int id);
        Task<PagedList<Recipe>> SearchRecipesAsync(string searchTerm, RecipeParams recipeParams);
        DbSet<RecipeCategory> RecipeCategories { get; }
        DbSet<Comment> Comments { get; }
        Task AddLikeAsync(Like like);
        void DeleteLike(Like like);
        Task AddCommentAsync(Comment comment);
        Task AddRecipeCategoryAsync(RecipeCategory recipeCategory);
        void DeleteRecipeCategory(RecipeCategory recipeCategory);
        Task AddCategoryAsync(Category category);
        void UpdateCategory(Category category);
        void DeleteCategory(Category category);
    }
}