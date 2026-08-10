using Core.Domain.Events;
using Core.EventBus.Consumer;
using DataIngestion.Services;
using Microsoft.Extensions.Logging;

namespace DataIngestion.Handlers;

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
