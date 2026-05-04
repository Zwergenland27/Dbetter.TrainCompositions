using System.Text.Json;
using CleanMessageBus.Abstractions;

namespace DBetter.TrainCompositions.Infrastructure.OutboxPattern;

public class OutboxMessage
{
    public Guid Id { get; private init; }
    
    public string Type { get; private init; }
    
    public string Payload { get; private init; }
    public DateTime OccuredAt { get; private init; }
    
    public DateTime? ProcessedAt { get; private set; }

    private OutboxMessage(Guid id, string type, string payload, DateTime occuredAt)
    {
        Id = id;
        Type = type;
        Payload = payload;
        OccuredAt = occuredAt;
    }

    private OutboxMessage(){}

    /// <summary>
    /// Create an <see cref="OutboxMessage"/> from an <see cref="IDomainEvent"/>
    /// </summary>
    /// <param name="event">Event that occured</param>
    /// <returns>OutboxMessage wrapper around event</returns>
    public static OutboxMessage FromEvent(IDomainEvent @event)
    {
        var type = @event.GetType();
        return new OutboxMessage(
            Guid.NewGuid(),
            $"{type.FullName}, {type.Assembly.GetName().Name}",
            JsonSerializer.Serialize(@event, @event.GetType()),
            DateTime.UtcNow);
    }

    /// <summary>
    /// Extract strongly typed event from outbox message
    /// </summary>
    /// <returns>DomainEvent, that is stored inside the wrapper</returns>
    public IDomainEvent ExtractEvent()
    {
        var type = System.Type.GetType(Type);
        return (IDomainEvent) JsonSerializer.Deserialize(Payload, type!)!;
    }

    /// <summary>
    /// Mark the outboxmessage as processed
    /// </summary>
    public void Processed()
    {
        ProcessedAt = DateTime.UtcNow;
    }
}