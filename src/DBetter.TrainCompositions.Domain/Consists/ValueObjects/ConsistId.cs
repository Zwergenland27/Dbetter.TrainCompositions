using CleanDomainValidation.Domain;

namespace DBetter.TrainCompositions.Domain.Consists.ValueObjects;

/// <summary>
/// Internal id of a consist
/// </summary>
public record ConsistId(Guid Value)
{
    /// <summary>
    /// Create a new Id
    /// </summary>
    /// <returns></returns>
    public static ConsistId CreateNew()
    {
        return new(Guid.CreateVersion7());
    }
    
    /// <summary>
    /// Validate and create a consist id from a string
    /// </summary>
    /// <param name="value">string representation of the guid</param>
    /// <exception cref="ConsistErrors.Id.Invalid">The id is no valid guid</exception>
    public static CanFail<ConsistId> Create(string value)
    {
        if (Guid.TryParse(value, out var guid))
        {
            return new ConsistId(guid);
        }

        return ConsistErrors.Id.Invalid;
    }
}