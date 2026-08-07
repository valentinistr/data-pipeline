using Core.ServiceBus.Consumer;
using Core.ServiceBus.Events;
using WorkerProcess.Services;

namespace WorkerProcess.Handlers;

public sealed class FileUploadEventConsumer(IDataIngestionService dataIngestionService) : IEventBusConsumer<FileUploadEvent>
{
    public Task ConsumeAsync(FileUploadEvent @event, CancellationToken cancellationToken)
    {
        Console.WriteLine($"FileUploadEvent: {@event.UploadLocation}, jobs={@event.JobsFileName ?? "(none)"}, employees={@event.EmployeesFileName ?? "(none)"}");

        return dataIngestionService.IngestAsync(@event, cancellationToken);
    }
}
