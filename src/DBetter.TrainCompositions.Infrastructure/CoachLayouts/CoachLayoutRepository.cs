using DBetter.TrainCompositions.Domain.CoachLayouts;
using DBetter.TrainCompositions.Domain.CoachLayouts.ValueObjects;
using DBetter.TrainCompositions.Infrastructure.OutboxPattern;
using DBetter.TrainCompositions.Infrastructure.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace DBetter.TrainCompositions.Infrastructure.CoachLayouts;

public class CoachLayoutRepository(DBetterContext db): ICoachLayoutRepository
{
    public void Store(CoachLayout aggregate)
    {
        var existing = db.CoachLayouts.Local.FirstOrDefault(coachLayout => coachLayout.Id == aggregate.Id.Value);
        if (existing is null)
        {
            db.CoachLayouts.Add(CoachLayoutPersistenceDto.FromDomain(aggregate));
        }
        else
        {
            existing.Apply(aggregate);
        }
        
        db.OutboxMessages.AddRange(aggregate.DomainEvents.Select(OutboxMessage.FromEvent));
    }

    public async Task<CoachLayout?> GetAsync(CoachLayoutId id)
    {
        var dto = await db.CoachLayouts.FirstOrDefaultAsync(coachLayout => coachLayout.Id == id.Value);
        return dto?.ToDomain();
    }

    public async Task<List<CoachLayout>> FindManyAsync(IEnumerable<CoachLayoutIdentifier> identifiers)
    {
        var dbFriendlyIdentifiers = identifiers.Select(identifier => identifier.Value);
        var dtos = await db.CoachLayouts
            .Where(coachLayout => dbFriendlyIdentifiers.Contains(coachLayout.Identifier))
            .ToListAsync();
        
        return dtos.Select(dto => dto.ToDomain()).ToList();
    }
}