using DiyetPlatform.Core.Interfaces;
using DiyetPlatform.Core.Interfaces.Repositories;
using DiyetPlatform.Infrastructure.Data.Context;
using DiyetPlatform.Infrastructure.Data.Repositories;
using DiyetPlatform.Infrastructure.Repositories;
using DiyetPlatform.Infrastructure.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DiyetPlatform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Register Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRecipeRepository, RecipeRepository>();
        services.AddScoped<IDietPlanRepository, DietPlanRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<ILikeRepository, LikeRepository>();
        services.AddScoped<IFollowRepository, FollowRepository>();

        // PostRepository requires IHostEnvironment
        services.AddScoped<IPostRepository>(provider => {
            var dbContext = provider.GetRequiredService<ApplicationDbContext>();
            var hostingEnvironment = provider.GetRequiredService<IHostEnvironment>();
            return new PostRepository(dbContext, hostingEnvironment);
        });

        // CategoryRepository requires IHostEnvironment
        services.AddScoped<ICategoryRepository>(provider => {
            var dbContext = provider.GetRequiredService<ApplicationDbContext>();
            var hostingEnvironment = provider.GetRequiredService<IHostEnvironment>();
            return new CategoryRepository(dbContext, hostingEnvironment);
        });

        // Register Services
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    public static async Task ApplyMigrationsAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();
            logger.LogError(ex, "An error occurred while applying migrations");
            throw;
        }
    }
}