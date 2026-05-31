using System.Diagnostics;
using System.Reflection.Emit;
using CleanDomainValidation.Domain;
using DBetter.TrainCompositions.Domain.Vehicles.ValueObjects;

namespace DBetter.TrainCompositions.Domain.Vehicles;

/// <summary>
/// Locomotive
/// </summary>
public class Locomotive: Vehicle
{
    /// <summary>
    /// The propulsion type of the locomotive
    /// </summary>
    public PropulsionType PropulsionType { get; private set; }
    
    internal Locomotive(VehicleId id, EuropeanVehicleNumber evn, float? maxSped, PropulsionType propulsionType) : base(id, evn, maxSped)
    {
        PropulsionType = propulsionType;
    }
    
    /// <summary>
    /// Creates a locomotive an european vehicle number
    /// </summary>
    /// <param name="evn">European vehicle number of the locomotive</param>
    /// <exception cref="VehicleErrors.Locomotive.InvalidEvn">The evn is not of type locomotive</exception>
    internal new static CanFail<Locomotive> Create(EuropeanVehicleNumber evn)
    {
        if (!evn.IsLocomotive)
        {
            return VehicleErrors.Locomotive.InvalidEvn;
        }

        var propulsionType = evn.VehicleTypeCode switch
        {
            EvnVehicleTypeCodes.Miscellaneous => PropulsionType.Hybrid,
            EvnVehicleTypeCodes.ElectricLocomotive => PropulsionType.Electric,
            EvnVehicleTypeCodes.ElectricShuntingLocomotive => PropulsionType.Electric,
            EvnVehicleTypeCodes.DieselLocomotive => PropulsionType.Diesel,
            EvnVehicleTypeCodes.DieselShuntingLocomotive => PropulsionType.Diesel,
            _ => throw new UnreachableException($"{evn.VehicleTypeCode} is no known power type")
        };

        return new Locomotive(VehicleId.CreateNew(), evn, null, propulsionType);
    }
}