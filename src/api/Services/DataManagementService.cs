using Core.EventBus.Events;
using Core.EventBus.Publisher;
using Core.Services;
using Core.Services.Storage;

namespace Api.Services;

public sealed class DataManagementService(
    IFileStorageService fileStorageService,
    IEventBusPublisher eventBus,
    IDataUploadsService dataUploadsService) : IDataManagementService
{
    private const string JobsPrefix = "jobs";
    private const string EmployeesPrefix = "employees";

    public async Task UploadAsync(
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

        var folderPath = await fileStorageService.SaveAsync(files, cancellationToken);
        var dataImport = await dataUploadsService.CreatePendingAsync(cancellationToken);

        await eventBus.PublishAsync(
            new FileUploadEvent
            {
                DataImportId = dataImport.Id,
                UploadLocation = folderPath,
                JobsFileName = jobs?.FileName,
                EmployeesFileName = employees?.FileName,
            },
            cancellationToken);
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
