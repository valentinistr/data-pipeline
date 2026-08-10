namespace Core.Services.Storage;

// Question: What is a record?
public sealed record UploadedFile(string FileName, Stream Content) : IDisposable, IAsyncDisposable
{
    public void Dispose()
    {
        Content.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await Content.DisposeAsync();
    }
}
