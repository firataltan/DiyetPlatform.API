using DiyetPlatform.API.Events;
using MassTransit;

namespace DiyetPlatform.API.Consumers
{
    public class UserCreatedEventConsumer : IConsumer<UserCreatedEvent>
    {
        private readonly ILogger<UserCreatedEventConsumer> _logger;

        public UserCreatedEventConsumer(ILogger<UserCreatedEventConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UserCreatedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation($"User created: {message.Email} at {message.CreatedAt}");
            
            // Burada kullanıcı oluşturulduğunda yapılacak işlemleri gerçekleştirebilirsiniz
            // Örneğin: Email gönderme, bildirim oluşturma, cache güncelleme vb.
            
            await Task.CompletedTask;
        }
    }
} 