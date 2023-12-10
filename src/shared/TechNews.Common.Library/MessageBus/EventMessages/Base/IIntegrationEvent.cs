namespace TechNews.Common.Library.MessageBus.EventMessages.Base;

public abstract class IntegrationEvent : Event
{
    protected IntegrationEvent(Guid aggregateId, string aggregateName, string eventName) : base(aggregateId, aggregateName, eventName)
    {
    }
}