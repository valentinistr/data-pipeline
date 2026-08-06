using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Server.ServiceBus.Consumer;

public sealed class EventBusConsumerWorker<TEvent>(
    EventBusConsumerOptions<TEvent> options,
    IServiceScopeFactory scopeFactory,
    ILogger<EventBusConsumerWorker<TEvent>> logger) : BackgroundService
    where TEvent : class
{
    private readonly string _channel = typeof(TEvent).FullName
                                       ?? throw new InvalidOperationException($"Could not resolve full name for type '{typeof(TEvent).Name}'.");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(
            "Event bus consumer started for {Channel} (poll every {PollInterval})",
            _channel,
            options.PollInterval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var batchHandler = scope.ServiceProvider.GetRequiredService<IEventBusMessageBatchHandler<TEvent>>();
                await batchHandler.HandleBatchAsync(stoppingToken);
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
}