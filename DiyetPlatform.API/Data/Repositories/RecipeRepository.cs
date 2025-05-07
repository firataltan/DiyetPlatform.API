using Microsoft.EntityFrameworkCore;
using DiyetPlatform.API.Helpers;
using DiyetPlatform.API.Models.Entities;

namespace DiyetPlatform.API.Data.Repositories
{
    public class RecipeRepository : GenericRepository<Recipe>, IRecipeRepository
    {
        public RecipeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public DbSet<RecipeCategory> RecipeCategories => _context.RecipeCategories;
        
        public DbSet<Comment> Comments => _context.Comments;
        
        public async Task AddLikeAsync(Like like)
        {
            await _context.Likes.AddAsync(like);
        }
        
        public void DeleteLike(Like like)
        {
            _context.Likes.Remove(like);
        }
        
        public async Task AddCommentAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
        }
        
        public async Task AddRecipeCategoryAsync(RecipeCategory recipeCategory)
        {
            await _context.RecipeCategories.AddAsync(recipeCategory);
        }

        public async Task<Recipe> GetRecipeByIdAsync(int id)
        {
            return await _context.Recipes
                .Include(r => r.User)
                    .ThenInclude(u => u.Profile)
                .Include(r => r.Likes)
                .Include(r => r.Comments)
                    .ThenInclude(c => c.User)
                        .ThenInclude(u => u.Profile)
                .Include(r => r.Categories)
                    .ThenInclude(rc => rc.Category)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<PagedList<Recipe>> GetRecipesAsync(RecipeParams recipeParams)
        {
            var query = _context.Recipes
                .Include(r => r.User)
                    .ThenInclude(u => u.Profile)
                .Include(r => r.Likes)
                .Include(r => r.Comments)
                .Include(r => r.Categories)
                    .ThenInclude(rc => rc.Category)
                .AsQueryable();

            if (recipeParams.CategoryId.HasValue)
            {
                query = query.Where(r => r.Categories.Any(rc => rc.CategoryId == recipeParams.CategoryId.Value));
            }

            query = recipeParams.OrderBy switch
            {
                "created" => query.OrderByDescending(r => r.CreatedAt),
                "updated" => query.OrderByDescending(r => r.UpdatedAt),
                "likes" => query.OrderByDescending(r => r.Likes.Count),
                "comments" => query.OrderByDescending(r => r.Comments.Count),
                "calories" => query.OrderByDescending(r => r.Calories),
                _ => query.OrderByDescending(r => r.CreatedAt)
            };

            return await PagedList<Recipe>.CreateAsync(query, recipeParams.PageNumber, recipeParams.PageSize);
        }

        public async Task<PagedList<Recipe>> GetUserRecipesAsync(int userId, RecipeParams recipeParams)
        {
            var query = _context.Recipes
                .Where(r => r.UserId == userId)
                .Include(r => r.User)
                    .ThenInclude(u => u.Profile)
                .Include(r => r.Likes)
                .Include(r => r.Comments)
                .Include(r => r.Categories)
                    .ThenInclude(rc => rc.Category)
                .AsQueryable();

            if (recipeParams.CategoryId.HasValue)
            {
                query = query.Where(r => r.Categories.Any(rc => rc.CategoryId == recipeParams.CategoryId.Value));
            }

            query = recipeParams.OrderBy switch
            {
                "created" => query.OrderByDescending(r => r.CreatedAt),
                "updated" => query.OrderByDescending(r => r.UpdatedAt),
                "likes" => query.OrderByDescending(r => r.Likes.Count),
                "comments" => query.OrderByDescending(r => r.Comments.Count),
                "calories" => query.OrderByDescending(r => r.Calories),
                _ => query.OrderByDescending(r => r.CreatedAt)
            };

            return await PagedList<Recipe>.CreateAsync(query, recipeParams.PageNumber, recipeParams.PageSize);
        }

        public async Task<PagedList<Recipe>> GetRecipesByCategoryAsync(int categoryId, RecipeParams recipeParams)
        {
            var query = _context.Recipes
                .Where(r => r.Categories.Any(rc => rc.CategoryId == categoryId))
                .Include(r => r.User)
                    .ThenInclude(u => u.Profile)
                .Include(r => r.Likes)
                .Include(r => r.Comments)
                .Include(r => r.Categories)
                    .ThenInclude(rc => rc.Category)
                .AsQueryable();

            query = recipeParams.OrderBy switch
            {
                "created" => query.OrderByDescending(r => r.CreatedAt),
                "updated" => query.OrderByDescending(r => r.UpdatedAt),
                "likes" => query.OrderByDescending(r => r.Likes.Count),
                "comments" => query.OrderByDescending(r => r.Comments.Count),
                "calories" => query.OrderByDescending(r => r.Calories),
                _ => query.OrderByDescending(r => r.CreatedAt)
            };

            return await PagedList<Recipe>.CreateAsync(query, recipeParams.PageNumber, recipeParams.PageSize);
        }

        public async Task<bool> IsRecipeExistsAsync(int id)
        {
            return await _context.Recipes.AnyAsync(r => r.Id == id);
        }

        public async Task<bool> IsUserOwnerOfRecipeAsync(int userId, int recipeId)
        {
            return await _context.Recipes.AnyAsync(r => r.Id == recipeId && r.UserId == userId);
        }

        public async Task<bool> IsRecipeLikedByUserAsync(int userId, int recipeId)
        {
            return await _context.Likes.AnyAsync(l => l.RecipeId == recipeId && l.UserId == userId);
        }

        public async Task<Like> GetRecipeLikeAsync(int userId, int recipeId)
        {
            return await _context.Likes
                .SingleOrDefaultAsync(l => l.RecipeId == recipeId && l.UserId == userId);
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task<PagedList<Recipe>> SearchRecipesAsync(string searchTerm, RecipeParams recipeParams)
        {
            var query = _context.Recipes
                .Include(r => r.User)
                    .ThenInclude(u => u.Profile)
                .Include(r => r.Likes)
                .Include(r => r.Comments)
                .Include(r => r.Categories)
                    .ThenInclude(rc => rc.Category)
                .Where(r => r.Title.Contains(searchTerm) ||
                           r.Description.Contains(searchTerm) ||
                           r.Ingredients.Contains(searchTerm) ||
                           r.Instructions.Contains(searchTerm))
                .AsQueryable();

            if (recipeParams.CategoryId.HasValue)
            {
                query = query.Where(r => r.Categories.Any(rc => rc.CategoryId == recipeParams.CategoryId.Value));
            }

            query = recipeParams.OrderBy switch
            {
                "created" => query.OrderByDescending(r => r.CreatedAt),
                "updated" => query.OrderByDescending(r => r.UpdatedAt),
                "likes" => query.OrderByDescending(r => r.Likes.Count),
                "comments" => query.OrderByDescending(r => r.Comments.Count),
                "calories" => query.OrderByDescending(r => r.Calories),
                _ => query.OrderByDescending(r => r.CreatedAt)
            };

            return await PagedList<Recipe>.CreateAsync(query, recipeParams.PageNumber, recipeParams.PageSize);
        }

        public void DeleteRecipeCategory(RecipeCategory recipeCategory)
        {
            _context.RecipeCategories.Remove(recipeCategory);
        }
    }
}