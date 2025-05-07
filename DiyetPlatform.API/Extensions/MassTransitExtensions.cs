using DiyetPlatform.API.Consumers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiyetPlatform.API.Extensions
{
    public static class MassTransitExtensions
    {
        public static IServiceCollection AddMassTransitServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(x =>
            {
                // Register consumers
                x.AddConsumer<UserCreatedEventConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    // Configure endpoints
                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
} 