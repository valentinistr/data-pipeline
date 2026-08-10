using Core.Data;
using Core.Domain.Events;
using Core.Domain.Models;
using Core.Services;
using Microsoft.Extensions.Logging;
using DataIngestion.DataProcessors;
using DataIngestion.Models;

namespace DataIngestion.Services;

public sealed class DataIngestionService(
    IDbContext dbContext,
    IDataProcessor<Job, JobCsvRow> jobDataProcessor,
    IDataProcessor<Employee, EmployeeCsvRow> employeeDataProcessor,
    IDataUploadsService dataUploadsService,
    ILogger<DataIngestionService> logger)
    : IDataIngestionService
{
    public async Task IngestAsync(FileUploadEvent @event, CancellationToken cancellationToken = default)
    {
        try
        {
            await dataUploadsService.SetProcessingAsync(@event.DataImportId, cancellationToken);

            var jobsResult = IngestJobs(@event);
            var employeesResult = IngestEmployees(@event);

            await dataUploadsService.SetCompletedAsync(
                @event.DataImportId,
                jobsResult.ValidRows,
                jobsResult.InvalidRows,
                employeesResult.ValidRows,
                employeesResult.InvalidRows,
                cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Data ingestion failed for DataImport {DataImportId} at {UploadLocation}",
                @event.DataImportId,
                @event.UploadLocation);

            await dataUploadsService.SetErrorAsync(@event.DataImportId, cancellationToken);
        }
    }

    private IngestionPackage<Job, JobCsvRow> IngestJobs(FileUploadEvent @event)
    {
        if (string.IsNullOrWhiteSpace(@event.JobsFileName))
        {
            return IngestionPackage<Job, JobCsvRow>.Empty;
        }

        var filePath = Path.Combine(@event.UploadLocation, Path.GetFileName(@event.JobsFileName));
        var jobs = jobDataProcessor.Ingest(filePath);

        foreach (var job in jobs.ValidData)
        {
            dbContext.Jobs.Add(job);
        }

        return jobs;
    }

    private IngestionPackage<Employee, EmployeeCsvRow> IngestEmployees(FileUploadEvent @event)
    {
        if (string.IsNullOrWhiteSpace(@event.EmployeesFileName))
        {
            return IngestionPackage<Employee, EmployeeCsvRow>.Empty;
        }

        var filePath = Path.Combine(@event.UploadLocation, Path.GetFileName(@event.EmployeesFileName));
        var employees = employeeDataProcessor.Ingest(filePath);

        foreach (var employee in employees.ValidData)
        {
            dbContext.Employees.Add(employee);
        }

        return employees;
    }
}
