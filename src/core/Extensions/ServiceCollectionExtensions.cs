using Microsoft.Extensions.DependencyInjection;
using Server.Services;

namespace Server.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddScoped<IDataUploadsService, DataUploadsService>();
        return services;
    }
}
