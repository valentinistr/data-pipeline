using Core.EventBus.Store;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Core.EventBus.Consumer;

public sealed class EventBusConsumerWorker<TEvent>(
    EventBusConsumerOptions<TEvent> options,
    InMemoryEventStore store,
    IServiceScopeFactory scopeFactory,
    ILogger<EventBusConsumerWorker<TEvent>> logger) : BackgroundService
    where TEvent : class
{
    private readonly string _channel = typeof(TEvent).FullName ?? typeof(TEvent).Name;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Event bus consumer started for {Channel} (poll every {PollInterval})", _channel, options.PollInterval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessWaitingMessagesAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled error while polling channel {Channel}", _channel);
            }

            try
            {
                await Task.Delay(options.PollInterval, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }
    }

    private async Task ProcessWaitingMessagesAsync(CancellationToken cancellationToken)
    {
        var waiting = store.DequeueAll<TEvent>();
        foreach (var @event in waiting)
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var consumer = scope.ServiceProvider.GetRequiredService<IEventBusConsumer<TEvent>>();
                await consumer.ConsumeAsync(@event, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to consume event on channel {Channel}", _channel);
            }
        }
    }
}
