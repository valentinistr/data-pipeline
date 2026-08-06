using Microsoft.Extensions.Logging;
using Server.ServiceBus.Consumer;
using Server.ServiceBus.Events;

namespace Consumer.Handlers;

public sealed class LogEventConsumer(ILogger<LogEventConsumer> logger) : IEventBusConsumer<LogEvent>
{
    public Task ConsumeAsync(LogEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "LogEvent at {Timestamp:o}: {Message}",
            @event.Timestamp,
            @event.Message);

        return Task.CompletedTask;
    }
}
