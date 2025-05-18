using DiyetPlatform.Core.Interfaces;
using DiyetPlatform.Core.Interfaces.Repositories;
using DiyetPlatform.Infrastructure.Data.Context;
using System;
using System.Threading.Tasks;

namespace DiyetPlatform.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private bool _disposed = false;
        
        public UnitOfWork(ApplicationDbContext context, 
                          IUserRepository users,
                          IPostRepository posts,
                          IRecipeRepository recipes,
                          ICategoryRepository categories,
                          IDietPlanRepository dietPlans,
                          INotificationRepository notifications,
                          ICommentRepository comments,
                          ILikeRepository likes,
                          IFollowRepository follows,
                          IRecipeCategoryRepository recipeCategories,
                          ISavedRecipeRepository savedRecipes)
        {
            _context = context;
            Users = users;
            Posts = posts;
            Recipes = recipes;
            Categories = categories;
            DietPlans = dietPlans;
            Notifications = notifications;
            Comments = comments;
            Likes = likes;
            Follows = follows;
            RecipeCategories = recipeCategories;
            SavedRecipes = savedRecipes;
        }

        public IUserRepository Users { get; }
        public IPostRepository Posts { get; }
        public IRecipeRepository Recipes { get; }
        public ICategoryRepository Categories { get; }
        public IDietPlanRepository DietPlans { get; }
        public INotificationRepository Notifications { get; }
        public ICommentRepository Comments { get; }
        public ILikeRepository Likes { get; }
        public IFollowRepository Follows { get; }
        public IRecipeCategoryRepository RecipeCategories { get; }
        public ISavedRecipeRepository SavedRecipes { get; }

        public async Task<bool> Complete()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }
    }
} 