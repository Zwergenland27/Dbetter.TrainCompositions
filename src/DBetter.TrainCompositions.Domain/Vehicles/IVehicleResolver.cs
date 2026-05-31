using DBetter.TrainCompositions.Domain.Vehicles.ValueObjects;

namespace DBetter.TrainCompositions.Domain.Vehicles;

/// <summary>
/// Methods to resolve vehicles
/// </summary>
public interface IVehicleResolver
{
    /// <summary>
    /// Resolve many vehicles
    /// </summary>
    /// <remarks>
    /// Not existing vehicles will be created and stored automatically
    /// </remarks>
    /// <param name="evns">European vehicle numbers that are searched</param>
    /// <returns>Vehicle aggregates for all requested identifiers</returns>
    Task<List<Vehicle>> ResolveManyAsync(List<EuropeanVehicleNumber> evns);
    
    /// <summary>
    /// List of all coach layouts that have been resolved by this resolver
    /// </summary>
    IReadOnlyList<Vehicle> AllKnownCoachLayouts { get; }
}