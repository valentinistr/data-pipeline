using Core.Domain.Models;
using Database.Data;
using Microsoft.EntityFrameworkCore;

namespace Database.Seeding;

internal sealed class InMemoryDatabaseSeed(AppDbContext dbContext) : IDatabaseSeed
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        if (await dbContext.Jobs.AnyAsync(cancellationToken))
        {
            return;
        }

        dbContext.Jobs.AddRange(
            new Job { JobCode = "ENG-01", Name = "Software Engineer" },
            new Job { JobCode = "ENG-02", Name = "Engineering Manager" },
            new Job { JobCode = "RES-01", Name = "Research Analyst" },
            new Job { JobCode = "HR-01", Name = "HR Specialist" });

        dbContext.Employees.AddRange(
            new Employee { EmployeeCode = "EMP-01", FirstName = "Ada", LastName = "Lovelace", Department = "Engineering", JobCode = "ENG-01" },
            new Employee { EmployeeCode = "EMP-02", FirstName = "Grace", LastName = "Hopper", Department = "Engineering", JobCode = "ENG-02" },
            new Employee { EmployeeCode = "EMP-03", FirstName = "Alan", LastName = "Turing", Department = "Research", JobCode = "RES-01" },
            new Employee { EmployeeCode = "EMP-04", FirstName = "Katherine", LastName = "Johnson", Department = "Research", JobCode = "RES-01" },
            new Employee { EmployeeCode = "EMP-05", FirstName = "Margaret", LastName = "Hamilton", Department = "Engineering", JobCode = "ENG-01" });

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
