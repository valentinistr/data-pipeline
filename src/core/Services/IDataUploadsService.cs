using Server.Models;

namespace Server.Services;

public interface IDataUploadsService
{
    Task<DataImport> CreateProcessingAsync(CancellationToken cancellationToken = default);

    Task SetCompletedAsync(
        int dataImportId,
        int validJobs,
        int invalidJobs,
        int validEmployees,
        int invalidEmployees,
        CancellationToken cancellationToken = default);

    Task SetErrorAsync(int dataImportId, CancellationToken cancellationToken = default);
}
