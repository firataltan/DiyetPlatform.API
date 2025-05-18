using DiyetPlatform.Core.Entities;
using DiyetPlatform.Core.Interfaces.Repositories;
using DiyetPlatform.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiyetPlatform.Infrastructure.Data.Repositories
{
    public class RecipeCategoryRepository : GenericRepository<RecipeCategory>, IRecipeCategoryRepository
    {
        public RecipeCategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<RecipeCategory> GetRecipeCategoryAsync(int recipeId, int categoryId)
        {
            return await _context.RecipeCategories
                .FirstOrDefaultAsync(rc => rc.RecipeId == recipeId && rc.CategoryId == categoryId);
        }

        public IQueryable<RecipeCategory> GetRecipeCategoriesQuery()
        {
            return _context.RecipeCategories
                .Include(rc => rc.Recipe)
                .Include(rc => rc.Category)
                .AsQueryable();
        }

        public async Task<IEnumerable<RecipeCategory>> GetRecipeCategoriesByRecipeIdAsync(int recipeId)
        {
            return await _context.RecipeCategories
                .Include(rc => rc.Category)
                .Where(rc => rc.RecipeId == recipeId)
                .ToListAsync();
        }

        public async Task<IEnumerable<RecipeCategory>> GetRecipesByCategoryIdAsync(int categoryId)
        {
            return await _context.RecipeCategories
                .Include(rc => rc.Recipe)
                .ThenInclude(r => r.User)
                .ThenInclude(u => u.Profile)
                .Where(rc => rc.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<bool> IsRecipeInCategoryAsync(int recipeId, int categoryId)
        {
            return await _context.RecipeCategories
                .AnyAsync(rc => rc.RecipeId == recipeId && rc.CategoryId == categoryId);
        }

        public new void Delete(RecipeCategory recipeCategory)
        {
            _context.RecipeCategories.Remove(recipeCategory);
        }
    }
}
