using Server.ServiceBus.Events;

namespace Consumer.Services;

public interface IDataIngestionService
{
    Task IngestAsync(FileUploadEvent @event, CancellationToken cancellationToken = default);
}