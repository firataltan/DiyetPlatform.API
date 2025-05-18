using DiyetPlatform.Core.Interfaces;
using DiyetPlatform.Core.Interfaces.Repositories;
using DiyetPlatform.Infrastructure.Data.Repositories;
using DiyetPlatform.Infrastructure.Repositories;
using DiyetPlatform.Infrastructure.Services;
using DiyetPlatform.Application.Interfaces;
using DiyetPlatform.Application.Services;
using DiyetPlatform.Application.Mappings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MassTransit;

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
            services.AddScoped<IDietPlanRepository, DietPlanRepository>();
            services.AddScoped<IRecipeRepository, RecipeRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IFollowRepository, FollowRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<ILikeRepository, LikeRepository>();
            services.AddScoped<IRecipeCategoryRepository, DiyetPlatform.Infrastructure.Data.Repositories.RecipeCategoryRepository>();            
            services.AddScoped<ISavedRecipeRepository, DiyetPlatform.Infrastructure.Data.Repositories.SavedRecipeRepository>();

            // PostRepository requires IHostEnvironment
            services.AddScoped<IPostRepository>(provider => {
                var dbContext = provider.GetRequiredService<Infrastructure.Data.Context.ApplicationDbContext>();
                var hostingEnvironment = provider.GetRequiredService<IHostEnvironment>();
                return new DiyetPlatform.Infrastructure.Data.Repositories.PostRepository(dbContext, hostingEnvironment);
            });

            // CategoryRepository requires IHostEnvironment
            services.AddScoped<ICategoryRepository>(provider => {
                var dbContext = provider.GetRequiredService<Infrastructure.Data.Context.ApplicationDbContext>();
                var hostingEnvironment = provider.GetRequiredService<IHostEnvironment>();
                return new DiyetPlatform.Infrastructure.Data.Repositories.CategoryRepository(dbContext, hostingEnvironment);
            });

            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            
            // Infrastructure Services
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ITokenService, TokenService>();

            // Application Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IDietPlanService, DietPlanService>();
            services.AddScoped<IRecipeService, RecipeService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ISearchService, SearchService>();

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SigningKey"])),
                        ValidateIssuer = true,
                        ValidIssuer = configuration["JWT:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = configuration["JWT:Audience"],
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            return services;
        }

        public static IServiceCollection AddMassTransitServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration["RabbitMQ:Host"], "/", h =>
                    {
                        h.Username(configuration["RabbitMQ:Username"]);
                        h.Password(configuration["RabbitMQ:Password"]);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}