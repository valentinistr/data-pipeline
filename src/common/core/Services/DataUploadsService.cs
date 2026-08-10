using Core.Data;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Services;

public sealed class DataUploadsService(IUnitOfWork unitOfWork) : IDataUploadsService
{
    public async Task<DataImport> CreatePendingAsync(CancellationToken cancellationToken = default)
    {
        var dataImport = new DataImport
        {
            Uploaded = DateTime.UtcNow,
            Status = "Pending",
        };

        unitOfWork.DataImports.Add(dataImport);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return dataImport;
    }

    public async Task SetProcessingAsync(int dataImportId, CancellationToken cancellationToken = default)
    {
        var dataImport = await GetDataUploadEntryAsync(dataImportId, cancellationToken);
        dataImport.Status = "Processing";

        await unitOfWork.SaveChangesAsync(cancellationToken);
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

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task SetErrorAsync(int dataImportId, CancellationToken cancellationToken = default)
    {
        unitOfWork.DiscardChanges();

        var dataImport = await GetDataUploadEntryAsync(dataImportId, cancellationToken);
        dataImport.Status = "Error";

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<DataImport> GetDataUploadEntryAsync(int dataImportId, CancellationToken cancellationToken)
    {
        return await unitOfWork.DataImports.Query
                   .FirstOrDefaultAsync(import => import.Id == dataImportId, cancellationToken)
               ?? throw new InvalidOperationException($"DataImport '{dataImportId}' was not found.");
    }
}
