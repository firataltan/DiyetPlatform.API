using DiyetPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiyetPlatform.Core.Interfaces.Repositories
{
    public interface IRecipeCategoryRepository : IGenericRepository<RecipeCategory>
    {
        Task<IEnumerable<RecipeCategory>> GetRecipeCategoriesByRecipeIdAsync(int recipeId);
        Task<IEnumerable<RecipeCategory>> GetRecipesByCategoryIdAsync(int categoryId);
        Task<RecipeCategory> GetRecipeCategoryAsync(int recipeId, int categoryId);
        IQueryable<RecipeCategory> GetRecipeCategoriesQuery();
        Task<bool> IsRecipeInCategoryAsync(int recipeId, int categoryId);
        new void Delete(RecipeCategory recipeCategory);
    }
} 