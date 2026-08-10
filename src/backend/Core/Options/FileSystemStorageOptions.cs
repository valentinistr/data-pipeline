namespace Core.Options;

public sealed class FileSystemStorageOptions
{
    public const string SectionName = "FileSystemStorage";
    public string BasePath { get; set; } = string.Empty;
}
