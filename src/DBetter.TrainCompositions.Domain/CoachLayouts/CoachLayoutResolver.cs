using DBetter.TrainCompositions.Domain.CoachLayouts.ValueObjects;

namespace DBetter.TrainCompositions.Domain.CoachLayouts;

/// <inheritdoc/>
public class CoachLayoutResolver(ICoachLayoutRepository repository): ICoachLayoutResolver
{
    private readonly List<CoachLayout> _knownCoachLayouts = [];

    internal CoachLayoutResolver(ICoachLayoutRepository repository, List<CoachLayout> knownCoachLayouts) : this(repository)
    {
        _knownCoachLayouts = knownCoachLayouts;
    }
    
    /// <inheritdoc/>
    public IReadOnlyList<CoachLayout> AllKnownCoachLayouts => _knownCoachLayouts.AsReadOnly();
    
    /// <inheritdoc/>
    public async Task<List<CoachLayout>> ResolveManyAsync(List<CoachLayoutIdentifier> coachIdentifier)
    {
        coachIdentifier = coachIdentifier.Distinct().ToList();
        var fromKnown = _knownCoachLayouts
            .Where(cl => coachIdentifier.Contains(cl.Identifier))
            .ToList();

        var stillMissing = coachIdentifier
            .Except(fromKnown.Select(c => c.Identifier));
        
        var fromRepository = await repository.FindManyAsync(stillMissing);
        _knownCoachLayouts.AddRange(fromRepository);

        var existing = fromKnown.Concat(fromRepository).ToList();
        
        var missingCoachLayouts = coachIdentifier
            .Where(identifier => existing.All(e => e.Identifier != identifier))
            .ToList();
        
        foreach (var missingCoachLayout in missingCoachLayouts)
        {
            var created = CoachLayout.CreateFromPlanned(missingCoachLayout);
            repository.Store(created);
            _knownCoachLayouts.Add(created);
            existing.Add(created);
        }
        
        return existing.ToList();
    }
}