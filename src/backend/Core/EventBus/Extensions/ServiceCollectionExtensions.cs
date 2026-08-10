using Core.EventBus.Consumer;
using Core.EventBus.Publisher;
using Core.EventBus.Store;
using Microsoft.Extensions.DependencyInjection;

namespace Core.EventBus.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventBus(this IServiceCollection services)
    {
        services.AddSingleton<InMemoryEventStore>();
        services.AddSingleton<IEventBusPublisher, InMemoryEventBusPublisher>();
        return services;
    }

    public static IServiceCollection AddEventBusConsumer<TEvent, TConsumer>(
        this IServiceCollection services,
        TimeSpan pollInterval)
        where TEvent : class
        where TConsumer : class, IEventBusConsumer<TEvent>
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(pollInterval, TimeSpan.Zero);

        services.AddSingleton(new EventBusConsumerOptions<TEvent> { PollInterval = pollInterval });
        services.AddScoped<IEventBusConsumer<TEvent>, TConsumer>();
        services.AddHostedService<EventBusConsumerWorker<TEvent>>();
        return services;
    }
}
