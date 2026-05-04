using CleanMessageBus.Abstractions;
using DBetter.TrainCompositions.Infrastructure.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace DBetter.TrainCompositions.Infrastructure.OutboxPattern;

/// <summary>
/// Process OutboxMessages
/// </summary>
[DisallowConcurrentExecution]
public class OutboxProcessor(
    IMessageBus bus,
    DBetterContext db) : IJob
{
    public static JobKey JobKey => new(nameof(OutboxProcessor));
    
    public async Task Execute(IJobExecutionContext context)
    {
        var messages = await db.OutboxMessages
            .Where(message => message.ProcessedAt == null)
            .Take(20)
            .ToListAsync();

        foreach (var message in messages)
        {
            IDomainEvent @event = message.ExtractEvent();
            await bus.PublishAsync(@event);
            message.Processed();
        }
        
        await db.SaveChangesAsync();
    }
}