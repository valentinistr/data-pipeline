using Api.Services;
using Core.Extensions;
using Core.ServiceBus;
using Core.Storage;
using Database.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSqlLiteDatabase(builder.Configuration);
builder.Services.AddEventBus();
builder.Services.AddFileStorage(builder.Configuration);
builder.Services.AddCoreServices();
builder.Services.AddScoped<IDataManagementService, DataManagementService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevClient", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseCors("DevClient");
}

if (!app.Environment.IsDevelopment() )
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
