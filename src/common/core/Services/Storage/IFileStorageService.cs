namespace Core.Services.Storage;

public interface IFileStorageService
{
    Task<string> SaveAsync(IReadOnlyCollection<UploadedFile> files, CancellationToken cancellationToken = default);
}
