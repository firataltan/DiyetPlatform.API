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
    public class SavedRecipeRepository : GenericRepository<SavedRecipe>, ISavedRecipeRepository
    {
        public SavedRecipeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<SavedRecipe> GetSavedRecipeAsync(int userId, int recipeId)
        {
            return await _context.SavedRecipes
                .FirstOrDefaultAsync(sr => sr.UserId == userId && sr.RecipeId == recipeId);
        }

        public IQueryable<SavedRecipe> GetSavedRecipesQuery()
        {
            return _context.SavedRecipes
                .Include(sr => sr.Recipe)
                .Include(sr => sr.User)
                .AsQueryable();
        }

        public async Task<bool> IsRecipeSavedByUserAsync(int userId, int recipeId)
        {
            return await _context.SavedRecipes
                .AnyAsync(sr => sr.UserId == userId && sr.RecipeId == recipeId);
        }

        public new void Delete(SavedRecipe savedRecipe)
        {
            _context.SavedRecipes.Remove(savedRecipe);
        }
    }
} 