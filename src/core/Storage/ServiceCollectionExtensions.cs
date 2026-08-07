using Core.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Storage;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFileStorage(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetSection(FileSystemStorageOptions.SectionName).Get<FileSystemStorageOptions>()
            ?? throw new InvalidOperationException($"Missing '{FileSystemStorageOptions.SectionName}' configuration section.");

        if (string.IsNullOrWhiteSpace(options.BasePath))
        {
            throw new InvalidOperationException($"{FileSystemStorageOptions.SectionName}:BasePath must be set.");
        }
        
        services.Configure<FileSystemStorageOptions>(configuration.GetSection(FileSystemStorageOptions.SectionName));
        services.AddScoped<IFileStorage, FileSystemStorage>();

        return services;
    }
}
