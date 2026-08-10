using Core.Options;
using Microsoft.Extensions.Options;

namespace Core.Services.Storage;

public sealed class FileSystemFileStorageService : IFileStorageService
{
    private readonly string _basePath;

    public FileSystemFileStorageService(IOptions<FileSystemStorageOptions> options)
    {
        var basePath = options.Value.BasePath;
        _basePath = !string.IsNullOrWhiteSpace(basePath)  
            ? basePath 
            : throw new ArgumentNullException(nameof(basePath));
    }
    
    public async Task<string> SaveAsync(
        IReadOnlyCollection<UploadedFile> files,
        CancellationToken cancellationToken = default)
    {
        if (files.Count == 0)
        {
            throw new ArgumentException("At least one file is required.", nameof(files));
        }

        var folderPath = Path.Combine(_basePath, Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(folderPath);

        foreach (var file in files)
        {
            await SaveFileAsync(file, folderPath, cancellationToken);
        }

        return folderPath;
    }

    private static async Task SaveFileAsync(UploadedFile file, string folderPath, CancellationToken cancellationToken)
    {
        var safeName = Path.GetFileName(file.FileName);
        if (string.IsNullOrWhiteSpace(safeName))
        {
            throw new ArgumentException("File name is invalid.", safeName);
        }

        var destination = Path.Combine(folderPath, safeName);
        await using var output = File.Create(destination);
        await file.Content.CopyToAsync(output, cancellationToken);
        await output.FlushAsync(cancellationToken);
    }
}
