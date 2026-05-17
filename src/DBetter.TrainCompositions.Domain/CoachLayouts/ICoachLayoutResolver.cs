using DBetter.TrainCompositions.Domain.CoachLayouts.ValueObjects;

namespace DBetter.TrainCompositions.Domain.CoachLayouts;

/// <summary>
/// Methods to resolve coaches
/// </summary>
public interface ICoachLayoutResolver
{
    /// <summary>
    /// Resolve many coaches
    /// </summary>
    /// <remarks>
    /// Not existing coach layouts will be created and stored automatically
    /// </remarks>
    /// <param name="coachIdentifier">Identifier, whose layouts are searched</param>
    /// <returns>Coach Layout aggregates for all requested identifiers</returns>
    Task<List<CoachLayout>> ResolveManyAsync(List<CoachLayoutIdentifier> coachIdentifier);
    
    /// <summary>
    /// List of all coach layouts that have been resolved by this resolver
    /// </summary>
    IReadOnlyList<CoachLayout> AllKnownCoachLayouts { get; }
}