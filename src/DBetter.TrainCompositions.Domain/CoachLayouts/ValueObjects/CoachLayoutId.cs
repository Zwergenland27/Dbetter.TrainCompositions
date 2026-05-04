using CleanDomainValidation.Domain;

namespace DBetter.TrainCompositions.Domain.CoachLayouts.ValueObjects;

/// <summary>
/// Internal id of a coach layout
/// </summary>
public record CoachLayoutId(Guid Value)
{
    /// <summary>
    /// Create a new Id
    /// </summary>
    /// <returns></returns>
    public static CoachLayoutId CreateNew()
    {
        return new(Guid.CreateVersion7());
    }

    /// <summary>
    /// Validate and create a coach layout id from a string
    /// </summary>
    /// <param name="value">string representation of the guid</param>
    public static CanFail<CoachLayoutId> Create(string value)
    {
        if (Guid.TryParse(value, out var guid))
        {
            return new CoachLayoutId(guid);
        }

        return CoachLayoutErrors.Id.Invalid;
    }
}