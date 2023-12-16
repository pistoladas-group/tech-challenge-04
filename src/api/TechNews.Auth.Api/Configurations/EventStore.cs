using EventStore.Client;

namespace TechNews.Auth.Api.Configurations;

public static class EventStore
{
    public static IServiceCollection ConfigureEventStore(this IServiceCollection services)
    {
        var settings = EventStoreClientSettings.Create(EnvironmentVariables.EventStoreConnectionString);
        
        services.AddSingleton(new EventStoreClient(settings));
        
        return services;
    }

}