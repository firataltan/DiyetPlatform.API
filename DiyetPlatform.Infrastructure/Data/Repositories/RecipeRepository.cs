using DiyetPlatform.Core.Entities;
using DiyetPlatform.Core.Interfaces.Repositories;
using DiyetPlatform.Core.Common;
using DiyetPlatform.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DiyetPlatform.Infrastructure.Data.Repositories
{
    public class RecipeRepository : GenericRepository<Recipe>, IRecipeRepository
    {
        public RecipeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Recipe> GetRecipeByIdAsync(int id)
        {
            return await _context.Recipes
                .Include(r => r.User)
                .ThenInclude(u => u.Profile)
                .Include(r => r.Categories)
                .ThenInclude(rc => rc.Category)
                .Include(r => r.Comments)
                .ThenInclude(c => c.User)
                .ThenInclude(u => u.Profile)
                .Include(r => r.Likes)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public IQueryable<Recipe> GetRecipesQuery()
        {
            return _context.Recipes
                .Include(r => r.User)
                .ThenInclude(u => u.Profile)
                .Include(r => r.Categories)
                .ThenInclude(rc => rc.Category)
                .Include(r => r.Comments)
                .Include(r => r.Likes)
                .AsQueryable();
        }

        public async Task<PagedList<Recipe>> GetRecipesAsync(RecipeParams recipeParams)
        {
            var query = _context.Recipes
                .Include(r => r.User)
                .ThenInclude(u => u.Profile)
                .Include(r => r.Categories)
                .ThenInclude(rc => rc.Category)
                .Include(r => r.Comments)
                .Include(r => r.Likes)
                .AsQueryable();

            if (recipeParams.CategoryId.HasValue)
            {
                query = query.Where(r => r.Categories.Any(rc => rc.CategoryId == recipeParams.CategoryId.Value));
            }

            if (!string.IsNullOrEmpty(recipeParams.Search))
            {
                query = query.Where(r => r.Title.Contains(recipeParams.Search) || 
                                         r.Description.Contains(recipeParams.Search) ||
                                         r.Ingredients.Contains(recipeParams.Search));
            }

            query = recipeParams.OrderBy switch
            {
                "created" => query.OrderByDescending(r => r.CreatedAt),
                _ => query.OrderByDescending(r => r.CreatedAt)
            };

            return PagedList<Recipe>.Create(query, recipeParams.PageNumber, recipeParams.PageSize);
        }

        public async Task<PagedList<Recipe>> GetRecipesByCategoryAsync(int categoryId, RecipeParams recipeParams)
        {
            var query = _context.RecipeCategories
                .Where(rc => rc.CategoryId == categoryId)
                .Select(rc => rc.Recipe)
                .Include(r => r.User)
                .ThenInclude(u => u.Profile)
                .Include(r => r.Categories)
                .ThenInclude(rc => rc.Category)
                .Include(r => r.Comments)
                .Include(r => r.Likes)
                .AsQueryable();

            if (!string.IsNullOrEmpty(recipeParams.Search))
            {
                query = query.Where(r => r.Title.Contains(recipeParams.Search) || 
                                         r.Description.Contains(recipeParams.Search) ||
                                         r.Ingredients.Contains(recipeParams.Search));
            }

            query = recipeParams.OrderBy switch
            {
                "created" => query.OrderByDescending(r => r.CreatedAt),
                _ => query.OrderByDescending(r => r.CreatedAt)
            };

            return PagedList<Recipe>.Create(query, recipeParams.PageNumber, recipeParams.PageSize);
        }

        public async Task<PagedList<Recipe>> SearchRecipesAsync(string searchTerm, RecipeParams recipeParams)
        {
            var query = _context.Recipes
                .Where(r => r.Title.Contains(searchTerm) || 
                           r.Description.Contains(searchTerm) || 
                           r.Ingredients.Contains(searchTerm) ||
                           r.Instructions.Contains(searchTerm))
                .Include(r => r.User)
                .ThenInclude(u => u.Profile)
                .Include(r => r.Categories)
                .ThenInclude(rc => rc.Category)
                .Include(r => r.Comments)
                .Include(r => r.Likes)
                .AsQueryable();

            if (recipeParams.CategoryId.HasValue)
            {
                query = query.Where(r => r.Categories.Any(rc => rc.CategoryId == recipeParams.CategoryId.Value));
            }

            query = recipeParams.OrderBy switch
            {
                "created" => query.OrderByDescending(r => r.CreatedAt),
                _ => query.OrderByDescending(r => r.CreatedAt)
            };

            return PagedList<Recipe>.Create(query, recipeParams.PageNumber, recipeParams.PageSize);
        }

        public async Task<PagedList<Recipe>> GetSavedRecipesAsync(int userId, RecipeParams recipeParams)
        {
            var query = _context.SavedRecipes
                .Where(sr => sr.UserId == userId)
                .Select(sr => sr.Recipe)
                .Include(r => r.User)
                .ThenInclude(u => u.Profile)
                .Include(r => r.Categories)
                .ThenInclude(rc => rc.Category)
                .Include(r => r.Comments)
                .Include(r => r.Likes)
                .AsQueryable();

            if (recipeParams.CategoryId.HasValue)
            {
                query = query.Where(r => r.Categories.Any(rc => rc.CategoryId == recipeParams.CategoryId.Value));
            }

            if (!string.IsNullOrEmpty(recipeParams.Search))
            {
                query = query.Where(r => r.Title.Contains(recipeParams.Search) || 
                                         r.Description.Contains(recipeParams.Search) ||
                                         r.Ingredients.Contains(recipeParams.Search));
            }

            query = recipeParams.OrderBy switch
            {
                "created" => query.OrderByDescending(r => r.CreatedAt),
                _ => query.OrderByDescending(r => r.CreatedAt)
            };

            return PagedList<Recipe>.Create(query, recipeParams.PageNumber, recipeParams.PageSize);
        }

        public async Task<PagedList<Recipe>> GetUserRecipesAsync(int userId, RecipeParams recipeParams)
        {
            var query = _context.Recipes
                .Where(r => r.UserId == userId)
                .Include(r => r.User)
                .ThenInclude(u => u.Profile)
                .Include(r => r.Categories)
                .ThenInclude(rc => rc.Category)
                .Include(r => r.Comments)
                .Include(r => r.Likes)
                .AsQueryable();

            if (recipeParams.CategoryId.HasValue)
            {
                query = query.Where(r => r.Categories.Any(rc => rc.CategoryId == recipeParams.CategoryId.Value));
            }

            if (!string.IsNullOrEmpty(recipeParams.Search))
            {
                query = query.Where(r => r.Title.Contains(recipeParams.Search) || 
                                         r.Description.Contains(recipeParams.Search) ||
                                         r.Ingredients.Contains(recipeParams.Search));
            }

            query = recipeParams.OrderBy switch
            {
                "created" => query.OrderByDescending(r => r.CreatedAt),
                _ => query.OrderByDescending(r => r.CreatedAt)
            };

            return PagedList<Recipe>.Create(query, recipeParams.PageNumber, recipeParams.PageSize);
        }

        public async Task<bool> IsRecipeExistsAsync(int id)
        {
            return await _context.Recipes.AnyAsync(r => r.Id == id);
        }

        public async Task<bool> IsUserOwnerOfRecipeAsync(int userId, int recipeId)
        {
            return await _context.Recipes.AnyAsync(r => r.Id == recipeId && r.UserId == userId);
        }

        public new void Delete(Recipe recipe)
        {
            _context.Recipes.Remove(recipe);
        }
    }
} 