using Database.Extensions;
using Database.Seeding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddSqlLiteDatabase(builder.Configuration);
builder.Services.AddSqlLiteSeedingServices();

using var host = builder.Build();
using var scope = host.Services.CreateScope();

var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Database.Seeding");
var databaseSeed = scope.ServiceProvider.GetRequiredService<IDatabaseSeed>();

logger.LogInformation("Seeding database...");
await databaseSeed.SeedAsync();
logger.LogInformation("Seeding finished.");

return 0;
