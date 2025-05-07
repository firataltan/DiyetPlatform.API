using DiyetPlatform.API.Data.Repositories;
using System.Threading.Tasks;

namespace DiyetPlatform.API.Data.UnitOfWork
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IPostRepository PostRepository { get; }
        IDietPlanRepository DietPlanRepository { get; }
        IRecipeRepository RecipeRepository { get; }
        INotificationRepository NotificationRepository { get; }
        IFollowRepository FollowRepository { get; }
        
        Task<bool> Complete();
        bool HasChanges();
    }
} 