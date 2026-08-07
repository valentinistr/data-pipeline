using Core.ServiceBus.Consumer;
using Core.ServiceBus.Events;
using Microsoft.Extensions.Logging;
using WorkerProcess.Services;

namespace WorkerProcess.Handlers;

public sealed class FileUploadEventConsumer(
    IDataIngestionService dataIngestionService,
    ILogger<FileUploadEventConsumer> logger) : IEventBusConsumer<FileUploadEvent>
{
    public Task ConsumeAsync(FileUploadEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "FileUploadEvent: {UploadLocation}, jobs={JobsFileName}, employees={EmployeesFileName}",
            @event.UploadLocation,
            @event.JobsFileName ?? "(none)",
            @event.EmployeesFileName ?? "(none)");

        return dataIngestionService.IngestAsync(@event, cancellationToken);
    }
}
