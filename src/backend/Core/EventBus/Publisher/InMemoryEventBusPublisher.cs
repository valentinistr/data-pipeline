using Core.EventBus.Store;

namespace Core.EventBus.Publisher;

public sealed class InMemoryEventBusPublisher(InMemoryEventStore store) : IEventBusPublisher
{
    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        ArgumentNullException.ThrowIfNull(@event);
        store.Enqueue(@event);
        return Task.CompletedTask;
    }
}
