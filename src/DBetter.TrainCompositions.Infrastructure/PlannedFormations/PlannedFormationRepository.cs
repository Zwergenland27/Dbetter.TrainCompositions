using DBetter.TrainCompositions.Domain.PlannedFormations;
using DBetter.TrainCompositions.Domain.PlannedFormations.ValueObjects;
using DBetter.TrainCompositions.Infrastructure.OutboxPattern;
using DBetter.TrainCompositions.Infrastructure.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace DBetter.TrainCompositions.Infrastructure.PlannedFormations;

public class PlannedFormationRepository(DBetterContext db): IPlannedFormationRepository
{
    public void Store(PlannedFormation aggregate)
    {
        var existing = db.PlannedFormations.Local.FirstOrDefault(coachLayout => coachLayout.Id == aggregate.Id.Value);
        if (existing is null)
        {
            db.PlannedFormations.Add(PlannedFormationPersistenceDto.FromDomain(aggregate));
        }
        else
        {
            existing.Apply(aggregate);
        }
        
        db.OutboxMessages.AddRange(aggregate.DomainEvents.Select(OutboxMessage.FromEvent));
    }

    public async Task<PlannedFormation?> GetAsync(PlannedFormationId id)
    {
        var dto = await db.PlannedFormations.FirstOrDefaultAsync(formation => formation.Id == id.Value);
        return dto?.ToDomain();
    }

    public async Task<List<PlannedFormation>> FindManyAsync(IEnumerable<PlannedFormationSnapshot> coachSequencesToFind)
    {
        var coachSequenceHashes = new List<string>();

        foreach (var plannedFormation in coachSequencesToFind)
        {
            coachSequenceHashes.Add(PlannedFormationPersistenceDto.ComputeCoachSequenceHash(plannedFormation.Coaches));
        }
        
        var dtos = await db.PlannedFormations.Where(formation =>
                coachSequenceHashes.Contains(formation.CoachSequenceHash) ||
                coachSequenceHashes.Contains(formation.ReverseCoachSequenceHash))
            .Distinct()
            .ToListAsync();
        return dtos.Select(dto => dto.ToDomain()).ToList();
    }
}