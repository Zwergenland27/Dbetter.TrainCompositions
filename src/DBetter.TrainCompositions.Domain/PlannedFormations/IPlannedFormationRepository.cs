using DBetter.TrainCompositions.Domain.Abstractions;
using DBetter.TrainCompositions.Domain.CoachLayouts.ValueObjects;
using DBetter.TrainCompositions.Domain.PlannedFormations.ValueObjects;

namespace DBetter.TrainCompositions.Domain.PlannedFormations;

/// <summary>
/// Repository for <see cref="PlannedFormation"/>
/// </summary>
public interface IPlannedFormationRepository: IRepository<PlannedFormation, PlannedFormationId>
{
    /// <summary>
    /// Tries to find an existing planned formations for the specified coach sequences
    /// </summary>
    /// <param name="coachSequencesToFind">ordered list of the coach layout ids</param>
    /// <returns>Planned formations that matches the coach sequences (reverse order will be checked as well)</returns>
    Task<List<PlannedFormation>> FindManyAsync(IEnumerable<PlannedFormationSnapshot> coachSequencesToFind);
}