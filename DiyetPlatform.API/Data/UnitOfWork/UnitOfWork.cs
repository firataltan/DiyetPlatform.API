using DiyetPlatform.API.Data.Repositories;
using System.Threading.Tasks;

namespace DiyetPlatform.API.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        
        public UnitOfWork(
            ApplicationDbContext context,
            IUserRepository userRepository,
            IPostRepository postRepository,
            IDietPlanRepository dietPlanRepository,
            IRecipeRepository recipeRepository,
            INotificationRepository notificationRepository,
            IFollowRepository followRepository)
        {
            _context = context;
            UserRepository = userRepository;
            PostRepository = postRepository;
            DietPlanRepository = dietPlanRepository;
            RecipeRepository = recipeRepository;
            NotificationRepository = notificationRepository;
            FollowRepository = followRepository;
        }

        public IUserRepository UserRepository { get; }
        public IPostRepository PostRepository { get; }
        public IDietPlanRepository DietPlanRepository { get; }
        public IRecipeRepository RecipeRepository { get; }
        public INotificationRepository NotificationRepository { get; }
        public IFollowRepository FollowRepository { get; }

        public async Task<bool> Complete()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }
    }
} 