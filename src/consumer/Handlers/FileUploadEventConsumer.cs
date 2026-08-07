using Server.ServiceBus.Consumer;
using Server.ServiceBus.Events;

namespace Consumer.Handlers;

public sealed class FileUploadEventConsumer : IEventBusConsumer<FileUploadEvent>
{
    public Task ConsumeAsync(FileUploadEvent @event, CancellationToken cancellationToken)
    {
        Console.WriteLine($"FileUploadEvent: {@event.UploadLocation}");
        return Task.CompletedTask;
    }
}
