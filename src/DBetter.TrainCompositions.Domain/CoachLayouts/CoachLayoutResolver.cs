using DBetter.TrainCompositions.Domain.CoachLayouts.ValueObjects;

namespace DBetter.TrainCompositions.Domain.CoachLayouts;

/// <summary>
/// Methods to resolve coaches
/// </summary>
public class CoachLayoutResolver(ICoachLayoutRepository repository)
{
    /// <summary>
    /// Resolve many coaches
    /// </summary>
    /// <remarks>
    /// Not existing coach layouts will be created and stored automatically
    /// </remarks>
    /// <param name="coachIdentifier">Identifier, whose layouts are searched</param>
    /// <returns>Coach Layout aggregates for all requested identifiers</returns>
    public async Task<List<CoachLayout>> ResolveMany(List<CoachLayoutIdentifier> coachIdentifier)
    {
        var existing = await repository.FindManyAsync(coachIdentifier);
        var missingCoachLayouts = coachIdentifier.Where(identifier => existing.All(e => e.Identifier != identifier));
        foreach (var missingCoachLayout in missingCoachLayouts)
        {
            var created = CoachLayout.CreateFromPlanned(missingCoachLayout);
            repository.Store(created);
            existing.Add(created);
        }
        
        return existing;
    }
}