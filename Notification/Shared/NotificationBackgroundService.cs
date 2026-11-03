using System.Text.Json;

namespace Zuhid.Notification.Shared;

public class NotificationBackgroundService(NotificationQueue queue, IServiceProvider serviceProvider, ILogger<NotificationBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Notification background worker started.");
        await foreach (var message in queue.DequeueMessages(stoppingToken))
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                await ProcessMessageAsync(scope.ServiceProvider, message, stoppingToken);
                logger.LogInformation($"Successfully processed {message.GetType().Name} : {JsonSerializer.Serialize(message, message.GetType())}");
            }
            catch (ValidatorException ex)
            {
                logger.LogWarning($"Validation failed for {message.GetType().Name}: {ex.Message}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to process {message.GetType().Name} : {JsonSerializer.Serialize(message, message.GetType())}");
            }
        }
        logger.LogInformation("Notification background worker stopping.");
    }

    private async Task ProcessMessageAsync(IServiceProvider serviceProvider, IMessage message,
        CancellationToken stoppingToken)
    {
        var consumerType = typeof(IConsumer<>).MakeGenericType(message.GetType());
        var consumer = serviceProvider.GetService(consumerType);
        if (consumer == null)
        {
            var concreteConsumerType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .FirstOrDefault(p => consumerType.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract);
            if (concreteConsumerType != null)
            {
                consumer = serviceProvider.GetRequiredService(concreteConsumerType);
            }
        }
        if (consumer == null)
        {
            throw new InvalidOperationException($"No consumer found for message type {message.GetType().Name}");
        }
        await ((dynamic)consumer).ConsumeAsync((dynamic)message, stoppingToken);
    }
}
