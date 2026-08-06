using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Server.Data;
using Server.ServiceBus.Models;

namespace Server.ServiceBus.Consumer;

public class EventBusMessageBatchHandler<TEvent>(
    IUnitOfWork unitOfWork,
    IEventBusConsumer<TEvent> consumer,
    ILogger<EventBusMessageBatchHandler<TEvent>> logger) : IEventBusMessageBatchHandler<TEvent> where TEvent : class
{
    private readonly string _channel = typeof(TEvent).FullName
                                       ?? throw new InvalidOperationException($"Could not resolve full name for type '{typeof(TEvent).Name}'.");

    public async Task HandleBatchAsync(CancellationToken cancellationToken)
    {
        var messages = await ClaimMessageBatchAsync(cancellationToken);
        
        foreach (var message in messages)
        {
            await HandleMessageAsync(message, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<List<EventBusMessage>> ClaimMessageBatchAsync(CancellationToken cancellationToken)
    {
        var messages = await unitOfWork.EventBusMessages.Query
            .Where(m => m.Channel == _channel && m.Status == EventBusMessageStatus.Pending)
            .OrderBy(m => m.Timestamp)
            .ThenBy(m => m.Id)
            .ToListAsync(cancellationToken);

        if (messages.Count == 0)
        {
            return messages;
        }

        foreach (var message in messages)
        {
            message.Status = EventBusMessageStatus.Processing;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogDebug(
            "Claimed {Count} message(s) on channel {Channel}",
            messages.Count,
            _channel);
        return messages;
    }

    private async Task HandleMessageAsync(EventBusMessage message, CancellationToken cancellationToken)
    {
        try
        {
            var payload = JsonSerializer.Deserialize<TEvent>(message.Payload)
                          ?? throw new InvalidOperationException(
                              $"Failed to deserialize payload for message {message.Id} on channel '{_channel}'.");

            await consumer.ConsumeAsync(payload, cancellationToken);

            message.Status = EventBusMessageStatus.Completed;
            message.CompletedTimestamp = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to consume message {MessageId} on channel {Channel}",
                message.Id,
                _channel);

            message.Status = EventBusMessageStatus.Failed;
            message.CompletedTimestamp = DateTime.UtcNow;
        }
    }
}