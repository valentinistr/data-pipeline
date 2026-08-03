namespace Server.ServiceBus.Consumer;

public interface IEventBusConsumer<in TEvent> where TEvent : class
{
    Task ConsumeAsync(TEvent @event, CancellationToken cancellationToken);
}
