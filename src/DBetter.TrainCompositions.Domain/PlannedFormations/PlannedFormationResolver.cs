using CleanDomainValidation.Domain;
using DBetter.TrainCompositions.Domain.CoachLayouts;
using DBetter.TrainCompositions.Domain.CoachLayouts.ValueObjects;

namespace DBetter.TrainCompositions.Domain.PlannedFormations;

/// <inheritdoc/>
public class PlannedFormationResolver(IPlannedFormationRepository repository): IPlannedFormationResolver
{
    private readonly List<PlannedFormation> _knownPlannedFormations = [];

    internal PlannedFormationResolver(IPlannedFormationRepository repository, List<PlannedFormation> plannedFormations)
        : this(repository)
    {
        _knownPlannedFormations = plannedFormations;
    }
    
    /// <inheritdoc/>
    public IReadOnlyList<PlannedFormation> AllKnownPlannedFormations => _knownPlannedFormations.AsReadOnly();
    
    /// <inheritdoc/>
    public async Task<List<PlannedFormation>> ResolveManyAsync(List<PlannedFormationSnapshot> coachSequences)
    {
        coachSequences = coachSequences.Distinct(new PlannedFormationSnapshotComparer()).ToList();
        var stillMissing = coachSequences.ToList();
        var fromKnown = new List<PlannedFormation>();

        foreach (var coachSequence in coachSequences)
        {
            var known = _knownPlannedFormations.FirstOrDefault(pf => pf.Matches(coachSequence));
            if (known is not null)
            {
                fromKnown.Add(known);
                stillMissing.Remove(coachSequence);
            }
        }
            
        var fromRepository = await repository.FindManyAsync(stillMissing);
        _knownPlannedFormations.AddRange(fromRepository);
        
        var existing = fromKnown.Concat(fromRepository).ToList();
        
        var unresolvedSequences = coachSequences
            .Where(sequence => existing.All(e => !e.Matches(sequence)))
            .ToList();

        foreach (var unresolvedSequence in unresolvedSequences)
        {
            var created = PlannedFormation.Create(unresolvedSequence);
            if (created.HasFailed) continue;
            repository.Store(created.Value);
            _knownPlannedFormations.Add(created.Value);
            existing.Add(created.Value);
        }

        return existing;
    }
}