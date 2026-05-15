using CleanDomainValidation.Domain;
using DBetter.TrainCompositions.Domain.CoachLayouts;
using DBetter.TrainCompositions.Domain.CoachLayouts.ValueObjects;

namespace DBetter.TrainCompositions.Domain.PlannedFormations;

/// <summary>
/// Methods to resolve planned formations
/// </summary>
public class PlannedFormationResolver(IPlannedFormationRepository repository, List<PlannedFormation> knownPlannedFormations)
{
    /// <summary>
    /// Resolve multiple planned formations
    /// </summary>
    /// <remarks>
    /// Not known planned formations will be created and stored automatically
    /// </remarks>
    /// <param name="coachSequences">List of the coach sequences of the formations</param>
    /// <returns>Planned formations for distinct requested coach sequences</returns>
    public async Task<List<PlannedFormation>> ResolveManyAsync(List<PlannedFormationSnapshot> coachSequences)
    {
        coachSequences = coachSequences.Distinct(new PlannedFormationSnapshotComparer()).ToList();
        var stillMissing = coachSequences.ToList();
        var fromKnown = new List<PlannedFormation>();

        foreach (var coachSequence in coachSequences)
        {
            var known = knownPlannedFormations.FirstOrDefault(pf => pf.Matches(coachSequence));
            if (known is not null)
            {
                fromKnown.Add(known);
                stillMissing.Remove(coachSequence);
            }
        }
            
        var fromRepository = await repository.FindManyAsync(stillMissing);
        knownPlannedFormations.AddRange(fromRepository);
        
        var existing = fromKnown.Concat(fromRepository).ToList();
        
        var unresolvedSequences = coachSequences
            .Where(sequence => existing.All(e => !e.Matches(sequence)))
            .ToList();

        foreach (var unresolvedSequence in unresolvedSequences)
        {
            var created = PlannedFormation.Create(unresolvedSequence);
            if (created.HasFailed) continue;
            repository.Store(created.Value);
            knownPlannedFormations.Add(created.Value);
            existing.Add(created.Value);
        }

        return existing;
    }
}