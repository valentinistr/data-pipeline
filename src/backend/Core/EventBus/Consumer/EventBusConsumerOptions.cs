namespace Core.EventBus.Consumer;

public sealed class EventBusConsumerOptions<TEvent> where TEvent : class
{
    public required TimeSpan PollInterval { get; init; }
}