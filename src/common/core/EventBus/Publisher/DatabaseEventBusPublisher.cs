using System.Text.Json;
using Core.Data;
using Core.EventBus.Models;

namespace Core.EventBus.Publisher;

public sealed class DatabaseEventBusPublisher(IUnitOfWork unitOfWork) : IEventBusPublisher
{
    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        ArgumentNullException.ThrowIfNull(@event);

        unitOfWork.EventBusMessages.Add(new EventBusMessage
        {
            Channel = typeof(TEvent).FullName ?? throw new InvalidOperationException($"Could not resolve full name for type '{typeof(TEvent).Name}'."),
            Payload = JsonSerializer.Serialize(@event),
            Timestamp = DateTime.UtcNow,
            Status = EventBusMessageStatus.Pending
        });

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
