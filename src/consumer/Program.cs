using Consumer.DataProcessors;
using Consumer.Handlers;
using Consumer.Models;
using Consumer.Services;
using Database.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Server.Extensions;
using Server.Models;
using Server.ServiceBus;
using Server.ServiceBus.Events;
using Server.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSqlLiteDatabase(builder.Configuration);
builder.Services.AddCoreServices();
builder.Services.AddScoped<IDataProcessor<Job, JobCsvRow>, JobDataProcessor>();
builder.Services.AddScoped<IDataProcessor<Employee, EmployeeCsvRow>, EmployeeDataProcessor>();
builder.Services.AddScoped<IDataIngestionService, DataIngestionService>();
builder.Services.AddEventBusConsumer<FileUploadEvent, FileUploadEventConsumer>(TimeSpan.FromSeconds(5));

var host = builder.Build();
await host.RunAsync();
