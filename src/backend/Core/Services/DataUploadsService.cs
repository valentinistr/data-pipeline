using Core.Data;
using Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Services;

public sealed class DataUploadsService(IDbContext dbContext) : IDataUploadsService
{
    public async Task<DataImport> CreatePendingAsync(CancellationToken cancellationToken = default)
    {
        var dataImport = new DataImport
        {
            Uploaded = DateTime.UtcNow,
            Status = "Pending",
        };

        dbContext.DataImports.Add(dataImport);
        await dbContext.SaveChangesAsync(cancellationToken);

        return dataImport;
    }

    public async Task SetProcessingAsync(int dataImportId, CancellationToken cancellationToken = default)
    {
        var dataImport = await GetDataUploadEntryAsync(dataImportId, cancellationToken);
        dataImport.Status = "Processing";

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SetCompletedAsync(
        int dataImportId,
        int validJobs,
        int invalidJobs,
        int validEmployees,
        int invalidEmployees,
        CancellationToken cancellationToken = default)
    {
        var dataImport = await GetDataUploadEntryAsync(dataImportId, cancellationToken);

        dataImport.Status = "Completed";
        dataImport.Completed = DateTime.UtcNow;
        dataImport.ValidJobs = validJobs;
        dataImport.InvalidJobs = invalidJobs;
        dataImport.ValidEmployees = validEmployees;
        dataImport.InvalidEmployees = invalidEmployees;

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SetErrorAsync(int dataImportId, CancellationToken cancellationToken = default)
    {
        dbContext.DiscardChanges();

        var dataImport = await GetDataUploadEntryAsync(dataImportId, cancellationToken);
        dataImport.Status = "Error";

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<DataImport> GetDataUploadEntryAsync(int dataImportId, CancellationToken cancellationToken)
    {
        return await dbContext.DataImports.Query
                   .FirstOrDefaultAsync(import => import.Id == dataImportId, cancellationToken)
               ?? throw new InvalidOperationException($"DataImport '{dataImportId}' was not found.");
    }
}
