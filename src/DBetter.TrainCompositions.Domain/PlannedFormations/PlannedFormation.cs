using CleanDomainValidation.Domain;
using DBetter.TrainCompositions.Domain.Abstractions;
using DBetter.TrainCompositions.Domain.PlannedFormations.Coaches;
using DBetter.TrainCompositions.Domain.PlannedFormations.Coaches.ValueObjects;
using DBetter.TrainCompositions.Domain.PlannedFormations.ValueObjects;

namespace DBetter.TrainCompositions.Domain.PlannedFormations;

/// <summary>
/// A planned coach formation
/// </summary>
public class PlannedFormation: AggregateRoot<PlannedFormationId>
{
    private readonly List<PlannedCoach> _coachSequence;
    
    /// <summary>
    /// Coach sequence of the planned formation
    /// </summary>
    public IReadOnlyCollection<PlannedCoach> CoachSequence =>  _coachSequence.OrderBy(coach => coach.Id.Value).ToList().AsReadOnly();
    
    internal PlannedFormation(PlannedFormationId id, List<PlannedCoach> coachSequence) : base(id)
    {
        _coachSequence = coachSequence;
    }

    /// <summary>
    /// Creates a new planned formation based on the specified coach layouts
    /// </summary>
    /// <param name="plannedFormationSnapshot">coach layouts of this planned formation</param>
    /// <exception cref="PlannedFormationErrors.TooShort">The coach list is empty</exception>
    internal static CanFail<PlannedFormation> Create(PlannedFormationSnapshot plannedFormationSnapshot)
    {
        var coachLayoutIds = plannedFormationSnapshot.Coaches;
        if (!coachLayoutIds.Any()) return PlannedFormationErrors.TooShort;
        
        var coaches = coachLayoutIds
            .Select((layoutId, index) => new PlannedCoach(new PlannedCoachPosition((short)index), layoutId))
            .ToList();
        
        return new PlannedFormation(PlannedFormationId.CreateNew(), coaches);
    }

    /// <summary>
    /// Checks weather the snapshot matches the coach sequence of the planned formation or not
    /// </summary>
    /// <param name="snapshot">coach layout that should be matched</param>
    /// <returns>True, if the snapshot matches</returns>
    public bool Matches(PlannedFormationSnapshot snapshot)
    {
        return CoachSequence.Zip(snapshot.Coaches).All(pair => pair.First.LayoutId == pair.Second);
    }
}