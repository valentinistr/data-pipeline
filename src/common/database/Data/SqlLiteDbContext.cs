using Core.EventBus.Models;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Data;

internal class SqlLiteDbContext(DbContextOptions<SqlLiteDbContext> options)
    : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<DataImport> DataImports => Set<DataImport>();
    public DbSet<EventBusMessage> EventBusMessages => Set<EventBusMessage>();
}
