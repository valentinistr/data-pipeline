namespace Core.ServiceBus.Consumer;

// ReSharper disable once UnusedTypeParameter
// Type parameter used for Dependency Injection
public sealed class EventBusConsumerOptions<TEvent> where TEvent : class
{
    public required TimeSpan PollInterval { get; init; }
}
