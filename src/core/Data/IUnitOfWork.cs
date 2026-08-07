using Core.Models;
using Core.ServiceBus.Models;

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
