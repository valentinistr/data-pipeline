namespace Core.ServiceBus.Events;

public sealed class FileUploadEvent
{
    public int DataImportId { get; set; }
    public string UploadLocation { get; set; } = string.Empty;
    public string? JobsFileName { get; set; }
    public string? EmployeesFileName { get; set; }
}
