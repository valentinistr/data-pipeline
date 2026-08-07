namespace Server.ServiceBus.Events;

public sealed class FileUploadEvent
{
    public string UploadLocation { get; set; } = string.Empty;
}
