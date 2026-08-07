using Core.ServiceBus.Events;

namespace WorkerProcess.Services;

public interface IDataIngestionService
{
    Task IngestAsync(FileUploadEvent @event, CancellationToken cancellationToken = default);
}