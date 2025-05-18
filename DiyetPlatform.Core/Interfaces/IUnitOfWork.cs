using DiyetPlatform.Core.Interfaces.Repositories;
using System;

namespace DiyetPlatform.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IPostRepository Posts { get; }
        IRecipeRepository Recipes { get; }
        ICategoryRepository Categories { get; }
        IDietPlanRepository DietPlans { get; }
        INotificationRepository Notifications { get; }
        ICommentRepository Comments { get; }
        ILikeRepository Likes { get; }
        IFollowRepository Follows { get; }
        IRecipeCategoryRepository RecipeCategories { get; }
        ISavedRecipeRepository SavedRecipes { get; }
         
        Task<bool> Complete();
        bool HasChanges();
    }
} 