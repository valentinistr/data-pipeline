using Microsoft.EntityFrameworkCore;
using Server.Models;
using Server.ServiceBus.Models;

namespace Database.Data;

public class SqlLiteDbContext(DbContextOptions<SqlLiteDbContext> options)
    : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<DataImport> DataImports => Set<DataImport>();
    public DbSet<EventBusMessage> EventBusMessages => Set<EventBusMessage>();
}
