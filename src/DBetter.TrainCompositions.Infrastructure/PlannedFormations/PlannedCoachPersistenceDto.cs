using DBetter.TrainCompositions.Domain.CoachLayouts.ValueObjects;
using DBetter.TrainCompositions.Domain.PlannedFormations.Coaches;
using DBetter.TrainCompositions.Domain.PlannedFormations.Coaches.ValueObjects;
using DBetter.TrainCompositions.Infrastructure.PostgreSQL;

namespace DBetter.TrainCompositions.Infrastructure.PlannedFormations;

public class PlannedCoachPersistenceDto: IPersistenceDto<PlannedCoach, PlannedCoachPersistenceDto>
{
    public required short Position { get; init; }
    
    public required Guid LayoutId { get; set; }
    
    public static PlannedCoachPersistenceDto FromDomain(PlannedCoach domain)
    {
        return new PlannedCoachPersistenceDto
        {
            Position = domain.Id.Value,
            LayoutId = domain.LayoutId.Value,
        };
    }

    public PlannedCoach ToDomain()
    {
        return new PlannedCoach(
            new PlannedCoachPosition(Position),
            new CoachLayoutId(LayoutId));
    }

    public void Apply(PlannedCoach domain)
    {
        LayoutId = domain.LayoutId.Value;
    }
}