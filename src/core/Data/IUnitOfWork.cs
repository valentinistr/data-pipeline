using Server.Models;
using Server.ServiceBus.Models;

namespace Server.Data;

public interface IUnitOfWork
{
    IRepository<Employee> Employees { get; }
    IRepository<Job> Jobs { get; }
    IRepository<DataImport> DataImports { get; }
    IRepository<EventBusMessage> EventBusMessages { get; }

    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
