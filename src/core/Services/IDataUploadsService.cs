using Core.Models;

namespace Core.Services;

public interface IDataUploadsService
{
    Task<DataImport> CreatePendingAsync(CancellationToken cancellationToken = default);

    Task SetProcessingAsync(int dataImportId, CancellationToken cancellationToken = default);

    Task SetCompletedAsync(
        int dataImportId,
        int validJobs,
        int invalidJobs,
        int validEmployees,
        int invalidEmployees,
        CancellationToken cancellationToken = default);

    Task SetErrorAsync(int dataImportId, CancellationToken cancellationToken = default);
}
