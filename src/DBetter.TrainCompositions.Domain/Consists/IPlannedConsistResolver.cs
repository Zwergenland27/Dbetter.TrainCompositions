using DBetter.TrainCompositions.Domain.Vehicles.ValueObjects;

namespace DBetter.TrainCompositions.Domain.Consists;

/// <summary>
/// Methods to resolve consists
/// </summary>
public interface IPlannedConsistResolver
{
    /// <summary>
    /// Resolve one consist
    /// </summary>
    /// <remarks>
    /// Not known consists will be created and stored automatically
    /// </remarks>
    /// <param name="vehicleIds">List of the vehicle ids of the consist</param>
    /// <returns>Consist for requested vehicle ids or null if the consist could not be created</returns>
    Task<Consist?> ResolveAsync(List<VehicleId> vehicleIds);
    
    /// <summary>
    /// List of all planned formations that have been resolved by this resolver
    /// </summary>
    IReadOnlyList<Consist> AllKnownConsists { get; }
}