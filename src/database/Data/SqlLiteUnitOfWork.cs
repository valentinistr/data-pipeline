using Core.Data;
using Core.Models;
using Core.ServiceBus.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Data;

public class SqlLiteUnitOfWork(SqlLiteDbContext dbContext) : IUnitOfWork
{
    public IRepository<Employee> Employees { get; } = new SqlLiteRepository<Employee>(dbContext.Employees);
    public IRepository<Job> Jobs { get; } = new SqlLiteRepository<Job>(dbContext.Jobs);
    public IRepository<DataImport> DataImports { get; } = new SqlLiteRepository<DataImport>(dbContext.DataImports);
    public IRepository<EventBusMessage> EventBusMessages { get; } = new SqlLiteRepository<EventBusMessage>(dbContext.EventBusMessages);

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
