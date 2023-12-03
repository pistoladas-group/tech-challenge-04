using MassTransit;
using TechNews.Services.Notification.Consumers;

namespace TechNews.Services.Notification.Configurations;

public static class MessageBroker
{
    public static IServiceCollection ConfigureMessageBroker(this IServiceCollection services)
    {
        //TODO: check if is possible to avoid having MassTransit creating extra queues with consumer name
        services.AddMassTransit(options =>
        {
            options.UsingRabbitMq((context, configuration) =>
            {
                configuration.Host(EnvironmentVariables.BrokerServer, EnvironmentVariables.BrokerVirtualHost, hostConfiguration =>
                {
                    hostConfiguration.Username(EnvironmentVariables.BrokerUserName);
                    hostConfiguration.Password(EnvironmentVariables.BrokerPassword);
                });
        
                configuration.ReceiveEndpoint(EnvironmentVariables.BrokerConfirmEmailQueueName, endpointConfiguration =>
                {
                    endpointConfiguration.Consumer<ConfirmEmailConsumer>();
                });
                
                configuration.ConfigureEndpoints(context);
            });
        
            options.AddConsumer<ConfirmEmailConsumer>();
        });

        return services;
    }
}