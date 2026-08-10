using Core.Options;
using Core.Services;
using Core.Services.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddScoped<IDataUploadsService, DataUploadsService>();
        return services;
    }
    
    public static IServiceCollection AddFileStorage(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetSection(FileSystemStorageOptions.SectionName).Get<FileSystemStorageOptions>()
                      ?? throw new InvalidOperationException($"Missing '{FileSystemStorageOptions.SectionName}' configuration section.");

        if (string.IsNullOrWhiteSpace(options.BasePath))
        {
            throw new InvalidOperationException($"{FileSystemStorageOptions.SectionName}:BasePath must be set.");
        }
        
        services.Configure<FileSystemStorageOptions>(configuration.GetSection(FileSystemStorageOptions.SectionName));
        services.AddScoped<IFileStorageService, FileSystemFileStorageService>();

        return services;
    }
}
