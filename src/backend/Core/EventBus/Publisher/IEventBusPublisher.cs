namespace Core.EventBus.Publisher;

public interface IEventBusPublisher
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class;
}
