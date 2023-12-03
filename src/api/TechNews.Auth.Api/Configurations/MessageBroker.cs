using MassTransit;

namespace TechNews.Auth.Api.Configurations;

public static class MessageBroker
{
    public static IServiceCollection ConfigureMessageBroker(this IServiceCollection services)
    {
        services.AddMassTransit(options =>
        {
            options.UsingRabbitMq((context, configuration) =>
            {
                configuration.Host(EnvironmentVariables.BrokerServer, EnvironmentVariables.BrokerVirtualHost, hostConfiguration =>
                {
                    hostConfiguration.Username(EnvironmentVariables.BrokerUserName);
                    hostConfiguration.Password(EnvironmentVariables.BrokerPassword);
                });

                configuration.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}