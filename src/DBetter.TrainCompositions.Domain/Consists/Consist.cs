using CleanDomainValidation.Domain;
using DBetter.TrainCompositions.Domain.Abstractions;
using DBetter.TrainCompositions.Domain.Consists.ValueObjects;
using DBetter.TrainCompositions.Domain.Consists.Vehicles;
using DBetter.TrainCompositions.Domain.Consists.Vehicles.ValueObjects;
using DBetter.TrainCompositions.Domain.Vehicles.ValueObjects;

namespace DBetter.TrainCompositions.Domain.Consists;

/// <summary>
/// A set of vehicles that are coupled as one train
/// </summary>
public class Consist: AggregateRoot<ConsistId>
{
    private List<CoupledVehicle> _vehicles;
    
    public IReadOnlyList<CoupledVehicle> Vehicles => _vehicles.OrderBy(v => v.Id).ToList().AsReadOnly();
    
    internal Consist(ConsistId id, List<CoupledVehicle> vehicles) : base(id)
    {
        _vehicles = vehicles;
    }
    
    /// <summary>
    /// Creates a new consist based on the specified vehicle ids
    /// </summary>
    /// <param name="vehicleIds">ids of the coupled vehicles</param>
    /// <exception cref="ConsistErrors.TooShort">The coach list is empty</exception>
    internal static CanFail<Consist> Create(List<VehicleId> vehicleIds)
    {
        if (!vehicleIds.Any()) return ConsistErrors.TooShort;
        
        var vehicles = vehicleIds
            .Select((vehicleId, index) => new CoupledVehicle(new CoupledVehiclePosition((short)index), vehicleId))
            .ToList();
        
        return new Consist(ConsistId.CreateNew(), vehicles);
    }

    /// <summary>
    /// Checks weather the provided vehicle id list matches the coupled vehicles of the consist
    /// </summary>
    /// <param name="vehicleIds">vehicle ids that should be matched</param>
    /// <returns>True, if the vehicle list matches</returns>
    public bool Matches(List<VehicleId> vehicleIds)
    {
        return Vehicles.Zip(vehicleIds).All(pair => pair.First.VehicleId == pair.Second);
    }
}