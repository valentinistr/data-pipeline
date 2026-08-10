using Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Data;

internal class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<DataImport> DataImports => Set<DataImport>();
}
