using Core.Domain.Events;

namespace DataIngestion.Services;

public interface IDataIngestionService
{
    Task IngestAsync(FileUploadEvent @event, CancellationToken cancellationToken = default);
}
