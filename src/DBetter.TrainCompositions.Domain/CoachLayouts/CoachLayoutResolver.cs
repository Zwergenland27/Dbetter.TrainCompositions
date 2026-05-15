using DBetter.TrainCompositions.Domain.CoachLayouts.ValueObjects;

namespace DBetter.TrainCompositions.Domain.CoachLayouts;

/// <summary>
/// Methods to resolve coaches
/// </summary>
public class CoachLayoutResolver(ICoachLayoutRepository repository, List<CoachLayout> knownCoachLayouts)
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
        coachIdentifier = coachIdentifier.Distinct().ToList();
        var fromKnown = knownCoachLayouts
            .Where(cl => coachIdentifier.Contains(cl.Identifier))
            .ToList();

        var stillMissing = coachIdentifier
            .Except(fromKnown.Select(c => c.Identifier));
        
        var fromRepository = await repository.FindManyAsync(stillMissing);
        knownCoachLayouts.AddRange(fromRepository);

        var existing = fromKnown.Concat(fromRepository).ToList();
        
        var missingCoachLayouts = coachIdentifier
            .Where(identifier => existing.All(e => e.Identifier != identifier))
            .ToList();
        
        foreach (var missingCoachLayout in missingCoachLayouts)
        {
            var created = CoachLayout.CreateFromPlanned(missingCoachLayout);
            repository.Store(created);
            knownCoachLayouts.Add(created);
            existing.Add(created);
        }
        
        return existing.ToList();
    }
}