using Server.Storage;

namespace Api.Services;

public sealed class DataManagementService(IFileStorage fileStorage) : IDataManagementService
{
    private const string JobsPrefix = "jobs";
    private const string EmployeesPrefix = "employees";

    public Task<string> UploadAsync(
        UploadedFile? jobs,
        UploadedFile? employees,
        CancellationToken cancellationToken = default)
    {
        ValidateFiles(jobs, employees);

        var files = new List<UploadedFile>(2);
        if (jobs is not null)
        {
            files.Add(jobs);
        }

        if (employees is not null)
        {
            files.Add(employees);
        }

        return fileStorage.SaveAsync(files, cancellationToken);
    }

    private static void ValidateFiles(UploadedFile? jobs, UploadedFile? employees)
    {
        if (jobs is null && employees is null)
        {
            throw new ArgumentException("At least one file is required.");
        }

        if (jobs is not null && !HasFileNamePrefix(jobs.FileName, JobsPrefix))
        {
            throw new ArgumentException("Jobs file name must start with 'jobs'.");
        }

        if (employees is not null && !HasFileNamePrefix(employees.FileName, EmployeesPrefix))
        {
            throw new ArgumentException("Employees file name must start with 'employees'.");
        }
    }

    private static bool HasFileNamePrefix(string fileName, string prefix)
    {
        var name = Path.GetFileName(fileName);
        return name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
    }
}
