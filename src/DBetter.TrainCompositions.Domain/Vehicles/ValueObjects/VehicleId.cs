using CleanDomainValidation.Domain;

namespace DBetter.TrainCompositions.Domain.Vehicles.ValueObjects;

/// <summary>
/// Internal id of a railway vehicle
/// </summary>
public record VehicleId(Guid Value)
{
    /// <summary>
    /// Create a new Id
    /// </summary>
    /// <returns></returns>
    public static VehicleId CreateNew()
    {
        return new(Guid.CreateVersion7());
    }
    
    /// <summary>
    /// Validate and create a planned vehicle id from a string
    /// </summary>
    /// <param name="value">string representation of the guid</param>
    /// <exception cref="VehicleErrors.Id.Invalid">The id is no valid guid</exception>
    public static CanFail<VehicleId> Create(string value)
    {
        if (Guid.TryParse(value, out var guid))
        {
            return new VehicleId(guid);
        }

        return VehicleErrors.Id.Invalid;
    }
}