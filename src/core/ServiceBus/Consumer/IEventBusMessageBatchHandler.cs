namespace Server.ServiceBus.Consumer;

public interface IEventBusMessageBatchHandler<TEvent> where TEvent : class
{
    Task HandleBatchAsync(CancellationToken cancellationToken);
}