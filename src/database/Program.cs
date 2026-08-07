using Database.Extensions;
using Database.Seeding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddSqlLiteDatabase(builder.Configuration);
builder.Services.AddSqlLiteSeedingServices();

using var host = builder.Build();
using var scope = host.Services.CreateScope();

var databaseSeed = scope.ServiceProvider.GetRequiredService<IDatabaseSeed>();

Console.WriteLine("Seeding database...");
await databaseSeed.SeedAsync();
Console.WriteLine("Seeding finished.");

return 0;
