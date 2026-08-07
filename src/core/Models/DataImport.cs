namespace Server.Models;

public class DataImport
{
    public int Id { get; set; }
    public DateTime Uploaded { get; set; }
    public DateTime? Completed { get; set; }
    public string Status { get; set; } = string.Empty;
    public int ValidEmployees { get; set; }
    public int InvalidEmployees { get; set; }
    public int ValidJobs { get; set; }
    public int InvalidJobs { get; set; }
}
