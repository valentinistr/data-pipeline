using Core.Data;
using Database.Data;
using Database.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Database.Extensions;

public static class InMemoryCollectionExtensions
{
    public static IServiceCollection AddInMemoryDatabase(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(db => db.UseInMemoryDatabase("data-pipeline"));
        services.AddScoped<IDbContext, InMemoryDataContext>();
        services.AddScoped<IDatabaseSeed, InMemoryDatabaseSeed>();
        return services;
    }

    public static async Task SeedInMemoryDatabaseAsync(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<IDatabaseSeed>().SeedAsync(cancellationToken);
    }
}
