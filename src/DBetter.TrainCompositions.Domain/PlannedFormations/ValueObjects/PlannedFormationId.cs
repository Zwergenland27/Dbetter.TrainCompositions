using CleanDomainValidation.Domain;

namespace DBetter.TrainCompositions.Domain.PlannedFormations.ValueObjects;


/// <summary>
/// Internal id of a planned formation
/// </summary>
public record PlannedFormationId(Guid Value)
{
    /// <summary>
    /// Create a new Id
    /// </summary>
    /// <returns></returns>
    public static PlannedFormationId CreateNew()
    {
        return new(Guid.CreateVersion7());
    }
    
    /// <summary>
    /// Validate and create a planned formation id from a string
    /// </summary>
    /// <param name="value">string representation of the guid</param>
    /// <exception cref="PlannedFormationErrors.Id.Invalid">The id is no valid guid</exception>
    public static CanFail<PlannedFormationId> Create(string value)
    {
        if (Guid.TryParse(value, out var guid))
        {
            return new PlannedFormationId(guid);
        }

        return PlannedFormationErrors.Id.Invalid;
    }
}