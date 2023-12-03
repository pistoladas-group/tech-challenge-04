using MassTransit;
using Serilog;
using TechNews.Common.Library.Models;

namespace TechNews.Services.Notification.Consumers;

public class ConfirmEmailConsumer : IConsumer<ConfirmEmailBrokerMessage>
{
    public Task Consume(ConsumeContext<ConfirmEmailBrokerMessage> context)
    {
        Log.Information(context.Message.ToString());
        return Task.CompletedTask;
    }
}