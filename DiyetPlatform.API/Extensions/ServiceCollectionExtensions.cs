using Microsoft.Extensions.DependencyInjection;
using DiyetPlatform.Application;

namespace DiyetPlatform.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // Use the Application layer's DependencyInjection to register services
            services.AddApplicationServices();
            return services;
        }
    }
} 