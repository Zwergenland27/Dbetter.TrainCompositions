using DBetter.TrainCompositions.Domain.Abstractions;
using DBetter.TrainCompositions.Domain.CoachLayouts.ValueObjects;

namespace DBetter.TrainCompositions.Domain.CoachLayouts;

/// <summary>
/// Repository
/// </summary>
public interface ICoachLayoutRepository: IRepository<CoachLayout, CoachLayoutId>
{
    /// <summary>
    /// Get many coach layouts based on a list of <see cref="CoachLayoutIdentifier"/>
    /// </summary>
    /// <param name="identifiers">Identifier, whose layouts are searched</param>
    /// <returns>List of all found coach layouts for the specified identifiers</returns>
    Task<List<CoachLayout>> FindManyAsync(IEnumerable<CoachLayoutIdentifier> identifiers);
}