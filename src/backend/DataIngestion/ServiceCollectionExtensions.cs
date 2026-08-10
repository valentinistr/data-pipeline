using Core.Domain.Events;
using Core.Domain.Models;
using Core.EventBus.Extensions;
using Microsoft.Extensions.DependencyInjection;
using DataIngestion.DataProcessors;
using DataIngestion.Handlers;
using DataIngestion.Models;
using DataIngestion.Services;

namespace DataIngestion;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataIngestion(this IServiceCollection services)
    {
        services.AddScoped<IDataProcessor<Job, JobCsvRow>, JobDataProcessor>();
        services.AddScoped<IDataProcessor<Employee, EmployeeCsvRow>, EmployeeDataProcessor>();
        services.AddScoped<IDataIngestionService, DataIngestionService>();
        services.AddEventBusConsumer<FileUploadEvent, FileUploadEventConsumer>(TimeSpan.FromSeconds(15));
        return services;
    }
}
