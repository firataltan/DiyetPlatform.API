using DiyetPlatform.API.Data.Repositories;
using DiyetPlatform.API.Helpers;
using DiyetPlatform.API.Services;
using DiyetPlatform.API.Data.UnitOfWork;

namespace DiyetPlatform.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            // AutoMapper
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

            // Repositories
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IDietPlanRepository, DietPlanRepository>();
            services.AddScoped<IRecipeRepository, RecipeRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IFollowRepository, FollowRepository>();

            // Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IDietPlanService, DietPlanService>();
            services.AddScoped<IRecipeService, RecipeService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ISearchService, SearchService>();

            // Helpers
            services.AddScoped<ITokenHelper, TokenHelper>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}