using Core.Extensions;
using Core.Models;
using Core.ServiceBus;
using Core.ServiceBus.Events;
using Database.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WorkerProcess.DataProcessors;
using WorkerProcess.Handlers;
using WorkerProcess.Models;
using WorkerProcess.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddSqlLiteDatabase(builder.Configuration);
builder.Services.AddCoreServices();
builder.Services.AddScoped<IDataProcessor<Job, JobCsvRow>, JobDataProcessor>();
builder.Services.AddScoped<IDataProcessor<Employee, EmployeeCsvRow>, EmployeeDataProcessor>();
builder.Services.AddScoped<IDataIngestionService, DataIngestionService>();
builder.Services.AddEventBusConsumer<FileUploadEvent, FileUploadEventConsumer>(TimeSpan.FromSeconds(15));

var host = builder.Build();
await host.RunAsync();
