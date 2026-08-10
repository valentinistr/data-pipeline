using Core.Data;
using Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Data;

internal class InMemoryDataContext(AppDbContext dbContext) : IDbContext
{
    public IRepository<Employee> Employees { get; } = new InMemoryRepository<Employee>(dbContext.Employees);
    public IRepository<Job> Jobs { get; } = new InMemoryRepository<Job>(dbContext.Jobs);
    public IRepository<DataImport> DataImports { get; } = new InMemoryRepository<DataImport>(dbContext.DataImports);

    public int SaveChanges() => dbContext.SaveChanges();

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);

    public void DiscardChanges()
    {
        foreach (var entry in dbContext.ChangeTracker.Entries().ToList())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.State = EntityState.Detached;
                    break;
                case EntityState.Modified:
                    entry.CurrentValues.SetValues(entry.OriginalValues);
                    entry.State = EntityState.Unchanged;
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Unchanged;
                    break;
            }
        }
    }
}
