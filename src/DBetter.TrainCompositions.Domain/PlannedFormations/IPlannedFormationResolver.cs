using DBetter.TrainCompositions.Domain.CoachLayouts;

namespace DBetter.TrainCompositions.Domain.PlannedFormations;

/// <summary>
/// Methods to resolve planned formations
/// </summary>
public interface IPlannedFormationResolver
{
    /// <summary>
    /// Resolve multiple planned formations
    /// </summary>
    /// <remarks>
    /// Not known planned formations will be created and stored automatically
    /// </remarks>
    /// <param name="coachSequences">List of the coach sequences of the formations</param>
    /// <returns>Planned formations for distinct requested coach sequences</returns>
    Task<List<PlannedFormation>> ResolveManyAsync(List<PlannedFormationSnapshot> coachSequences);
    
    /// <summary>
    /// List of all planned formations that have been resolved by this resolver
    /// </summary>
    IReadOnlyList<PlannedFormation> AllKnownPlannedFormations { get; }
}