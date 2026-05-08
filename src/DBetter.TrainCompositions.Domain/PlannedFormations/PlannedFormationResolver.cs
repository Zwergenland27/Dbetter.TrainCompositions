using CleanDomainValidation.Domain;
using DBetter.TrainCompositions.Domain.CoachLayouts;
using DBetter.TrainCompositions.Domain.CoachLayouts.ValueObjects;

namespace DBetter.TrainCompositions.Domain.PlannedFormations;

/// <summary>
/// Methods to resolve planned formations
/// </summary>
public class PlannedFormationResolver(IPlannedFormationRepository repository)
{
    /// <summary>
    /// Resolve multiple planned formations
    /// </summary>
    /// <remarks>
    /// Not known planned formations will be created and stored automatically
    /// </remarks>
    /// <param name="coachSequences">List of the coach sequences of the formations</param>
    /// <returns>Planned formations for all requested coach sequences</returns>
    public async Task<List<PlannedFormation>> ResolveManyAsync(List<PlannedFormationSnapshot> coachSequences)
    {
        var existing = await repository.FindManyAsync(coachSequences);
        var unresolvedSequences = coachSequences
            .Where(sequence => existing.All(e => !e.Matches(sequence)))
            .ToList();

        foreach (var unresolvedSequence in unresolvedSequences)
        {
            var created = PlannedFormation.Create(unresolvedSequence);
            if (created.HasFailed) continue;
            repository.Store(created.Value);
            existing.Add(created.Value);
        }

        return existing;
    }
}