using DBetter.TrainCompositions.Domain.Abstractions;
using DBetter.TrainCompositions.Domain.Consists.Vehicles.ValueObjects;
using DBetter.TrainCompositions.Domain.Vehicles.ValueObjects;

namespace DBetter.TrainCompositions.Domain.Consists.Vehicles;

/// <summary>
/// Vehicle as part of the consist
/// </summary>
public class CoupledVehicle: Entity<CoupledVehiclePosition>
{
    /// <summary>
    /// Id of the vehicle 
    /// </summary>
    public VehicleId VehicleId { get; init; }
    
    internal CoupledVehicle(CoupledVehiclePosition id, VehicleId vehicleId) : base(id)
    {
        VehicleId = vehicleId;
    }
}