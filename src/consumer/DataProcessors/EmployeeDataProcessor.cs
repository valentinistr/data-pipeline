using Consumer.Models;
using Server.Models;

namespace Consumer.DataProcessors;

public class EmployeeDataProcessor : BaseDataProcessor<Employee, EmployeeCsvRow>
{
    protected override bool ValidateRow(EmployeeCsvRow row) => !string.IsNullOrWhiteSpace(row.EmployeeCode) && !string.IsNullOrWhiteSpace(row.JobCode);

    protected override Employee MapRow(EmployeeCsvRow row) => new()
    {
        EmployeeCode = row.EmployeeCode.Trim(),
        JobCode = row.JobCode.Trim(),
        FirstName = row.FirstName?.Trim() ?? string.Empty,
        LastName = row.LastName?.Trim() ?? string.Empty,
        Department = row.Department?.Trim() ?? string.Empty,
    };
}