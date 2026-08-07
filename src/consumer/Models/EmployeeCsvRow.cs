namespace Consumer.Models;

public sealed class EmployeeCsvRow
{
    public string EmployeeCode { get; set; } = string.Empty;
    public string JobCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
}
