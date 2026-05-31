using DBetter.TrainCompositions.Domain.Abstractions;
using DBetter.TrainCompositions.Domain.Vehicles.ValueObjects;

namespace DBetter.TrainCompositions.Domain.Vehicles;

/// <summary>
/// Repository for <see cref="Vehicle"/>
/// </summary>
public interface IVehicleRepository: IRepository<Vehicle, VehicleId>
{
    /// <summary>
    /// Retrieves the specified vehicles by their european vehicle number
    /// </summary>
    /// <param name="evns">List of the evns that should be retrieved</param>
    /// <returns>All matching vehicles</returns>
    Task<List<Vehicle>> GetManyAsync(IEnumerable<EuropeanVehicleNumber> evns);
}