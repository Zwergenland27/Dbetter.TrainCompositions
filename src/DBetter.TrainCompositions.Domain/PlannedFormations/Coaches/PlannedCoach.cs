using DBetter.TrainCompositions.Domain.Abstractions;
using DBetter.TrainCompositions.Domain.CoachLayouts.ValueObjects;
using DBetter.TrainCompositions.Domain.PlannedFormations.Coaches.ValueObjects;

namespace DBetter.TrainCompositions.Domain.PlannedFormations.Coaches;

/// <summary>
/// A coach of <see cref="PlannedFormation"/>
/// </summary>
public class PlannedCoach: Entity<PlannedCoachPosition>
{
    /// <summary>
    /// Id of the coach layout with furhter information
    /// </summary>
    public CoachLayoutId LayoutId { get; init; }
    
    internal PlannedCoach(PlannedCoachPosition id, CoachLayoutId layoutId) : base(id)
    {
        LayoutId = layoutId;
    }
}