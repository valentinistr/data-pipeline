using System.Text.Json;
using Core.Data;
using Core.ServiceBus.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Core.ServiceBus.Consumer;

public sealed class EventBusConsumerWorker<TEvent>(
    EventBusConsumerOptions<TEvent> options,
    IUnitOfWork unitOfWork,
    IServiceScopeFactory scopeFactory,
    ILogger<EventBusConsumerWorker<TEvent>> logger) : BackgroundService
    where TEvent : class
{
    private readonly string _channel = typeof(TEvent).FullName ?? throw new InvalidOperationException($"Could not resolve full name for type '{typeof(TEvent).Name}'.");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Event bus consumer started for {Channel} (poll every {PollInterval})", _channel, options.PollInterval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessNextMessageAsync(stoppingToken);
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

    private async Task ProcessNextMessageAsync(CancellationToken cancellationToken)
    {
        var message = await ClaimNextMessageAsync(cancellationToken);
        if (message is null)
        {
            return;
        }

        try
        {
            var payload = JsonSerializer.Deserialize<TEvent>(message.Payload) ?? throw new InvalidOperationException($"Failed to deserialize payload for message {message.Id} on channel '{_channel}'.");

            await using (var scope = scopeFactory.CreateAsyncScope())
            {
                var consumer = scope.ServiceProvider.GetRequiredService<IEventBusConsumer<TEvent>>();
                await consumer.ConsumeAsync(payload, cancellationToken);
            }
            
            message.Status = EventBusMessageStatus.Completed;
            message.CompletedTimestamp = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to consume message {MessageId} on channel {Channel}", message.Id, _channel);

            message.Status = EventBusMessageStatus.Failed;
            message.CompletedTimestamp = DateTime.UtcNow;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<EventBusMessage?> ClaimNextMessageAsync( CancellationToken cancellationToken)
    {
        var message = await unitOfWork.EventBusMessages.Query
            .Where(m => m.Channel == _channel && m.Status == EventBusMessageStatus.Pending)
            .OrderBy(m => m.Timestamp)
            .ThenBy(m => m.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (message is null)
        {
            return null;
        }

        message.Status = EventBusMessageStatus.Processing;
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogDebug("Claimed message {MessageId} on channel {Channel}", message.Id, _channel);
        return message;
    }
}
