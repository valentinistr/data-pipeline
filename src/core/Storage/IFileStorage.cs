namespace Server.Storage;

public interface IFileStorage
{
    Task<string> SaveAsync(IReadOnlyCollection<UploadedFile> files, CancellationToken cancellationToken = default);
}
