using Core.Domain.Models;

namespace Core.Data;

public interface IDbContext
{
    IRepository<Employee> Employees { get; }
    IRepository<Job> Jobs { get; }
    IRepository<DataImport> DataImports { get; }

    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    void DiscardChanges();
}
