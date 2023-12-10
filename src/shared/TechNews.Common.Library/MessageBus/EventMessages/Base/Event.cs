namespace TechNews.Common.Library.MessageBus.EventMessages.Base;

public abstract class Event : Message
{
    public string EventName { get; set; }

    protected Event(Guid aggregateId, string aggregateName, string eventName) : base(aggregateId, aggregateName)
    {
        EventName = eventName;
    }
}