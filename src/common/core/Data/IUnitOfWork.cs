using Core.EventBus.Models;
using Core.Models;

namespace Core.Data;

public interface IUnitOfWork
{
    IRepository<Employee> Employees { get; }
    IRepository<Job> Jobs { get; }
    IRepository<DataImport> DataImports { get; }
    IRepository<EventBusMessage> EventBusMessages { get; }

    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    void DiscardChanges();
}
