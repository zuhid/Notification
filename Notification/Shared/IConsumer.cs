namespace Zuhid.Notification.Shared;

public interface IConsumer<T> where T : IMessage
{
    Task ConsumeAsync(T message, CancellationToken stoppingToken);
}
