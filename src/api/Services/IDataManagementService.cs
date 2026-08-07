using Server.Storage;

namespace Api.Services;

public interface IDataManagementService
{
    Task<string> UploadAsync(UploadedFile? jobs, UploadedFile? employees, CancellationToken cancellationToken = default);
}
