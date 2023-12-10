namespace TechNews.Common.Library.MessageBus.EventMessages.Base;

public abstract class Message
{
    public Guid AggregateId { get; set; }
    public string AggregateName { get; set; }
    public DateTime TimeStamp { get; set; }

    protected Message(Guid aggregateId, string aggregateName)
    {
        AggregateId = aggregateId;
        AggregateName = aggregateName;
        TimeStamp = DateTime.UtcNow;
    }
}