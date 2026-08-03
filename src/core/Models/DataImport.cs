namespace Server.Models;

public class DataImport
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public int ValidEmployees { get; set; }
    public int InvalidEmployees { get; set; }
    public int ValidJobs { get; set; }
    public int InvalidJobs { get; set; }
}
