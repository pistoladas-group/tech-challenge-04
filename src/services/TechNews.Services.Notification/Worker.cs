using Serilog;
using TechNews.Common.Library.MessageBus;
using TechNews.Common.Library.Messages.Events;
using TechNews.Services.Notification.Configurations;

namespace TechNews.Services.Notification;

public class Worker : BackgroundService
{
    private readonly IMessageBus _bus;

    public Worker(IMessageBus bus)
    {
        _bus = bus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _bus.Consume<UserRegisteredEvent>(EnvironmentVariables.BrokerConfirmEmailQueueName, ExecuteAfterConsumed);
    }

    public void ExecuteAfterConsumed(UserRegisteredEvent? message)
    {
        // TODO: Disparar email de confirmação com o Token
        Log.Debug("Message Consumed", DateTimeOffset.Now);
    }
}
