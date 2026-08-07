using Core.Data;
using Core.Options;
using Database.Data;
using Database.Seeding;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Database.Extensions;

public static class SqlLiteCollectionExtensions
{
    public static IServiceCollection AddSqlLiteDatabase( this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>()
            ?? throw new InvalidOperationException($"Missing '{DatabaseOptions.SectionName}' configuration section.");

        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            throw new InvalidOperationException($"{DatabaseOptions.SectionName}:ConnectionString must be set.");
        }

        EnsureDatabaseFileLocation(options.ConnectionString);
        
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));
        services.AddDbContext<SqlLiteDbContext>(db => db.UseSqlite(options.ConnectionString));
        services.AddScoped<IUnitOfWork, SqlLiteUnitOfWork>();

        return services;
    }

    internal static IServiceCollection AddSqlLiteSeedingServices(this IServiceCollection services)
    {
        services.AddSingleton<IDatabaseSeed, SqlLiteDatabaseSeed>();
        return services;
    }

    private static void EnsureDatabaseFileLocation(string optionsConnectionString)
    {
        var builder = new SqliteConnectionStringBuilder(optionsConnectionString);
        var dataSource = builder.DataSource;

        if (string.IsNullOrWhiteSpace(dataSource) || dataSource == ":memory:")
        {
            return;
        }

        var fullPath = Path.IsPathRooted(dataSource)
            ? dataSource
            : Path.GetFullPath(dataSource);

        var directory = Path.GetDirectoryName(fullPath);

        if (string.IsNullOrWhiteSpace(directory))
        {
            return;
        }

        Directory.CreateDirectory(directory);
    }
}
