using TechNews.Common.Library.MessageBus.EventMessages.Base;

namespace TechNews.Common.Library.MessageBus;

public interface IMessageBus
{
    public void Publish<T>(T message);
    public void Consume<T>(string queueName, Action<T?> executeAfterConsumed) where T : IntegrationEvent;
}