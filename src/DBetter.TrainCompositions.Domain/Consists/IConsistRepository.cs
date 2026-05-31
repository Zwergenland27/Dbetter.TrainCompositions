using DBetter.TrainCompositions.Domain.Abstractions;
using DBetter.TrainCompositions.Domain.Consists.ValueObjects;
using DBetter.TrainCompositions.Domain.Vehicles.ValueObjects;

namespace DBetter.TrainCompositions.Domain.Consists;

/// <summary>
/// Repository for <see cref="Consist"/>
/// </summary>
public interface IConsistRepository: IRepository<Consist, ConsistId>
{
    /// <summary>
    /// Tries to find an existing consist that matches the list of vehicle ids
    /// </summary>
    /// <param name="vehicleIds">ordered list of the vehicle ids</param>
    /// <returns>Consist that matches the list of vehicle ids (reverse order will be checked as well)</returns>
    Task<Consist?> FindAsync(List<VehicleId> vehicleIds);
}