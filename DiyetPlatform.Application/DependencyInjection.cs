using DiyetPlatform.Application.Services;
using DiyetPlatform.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;

namespace DiyetPlatform.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Add AutoMapper
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // Add FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Register Application Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<IRecipeService, RecipeService>();
        services.AddScoped<IDietPlanService, DietPlanService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<ISearchService, SearchService>();

        return services;
    }
} 