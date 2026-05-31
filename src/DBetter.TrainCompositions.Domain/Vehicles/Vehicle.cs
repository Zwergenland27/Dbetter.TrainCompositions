using CleanDomainValidation.Domain;
using DBetter.TrainCompositions.Domain.Abstractions;
using DBetter.TrainCompositions.Domain.Vehicles.ValueObjects;

namespace DBetter.TrainCompositions.Domain.Vehicles;

/// <summary>
/// A railway wagon or locomotive
/// </summary>
public abstract class Vehicle: AggregateRoot<VehicleId>
{
    /// <summary>
    /// Unique european vehicle number of the vehicle
    /// </summary>
    /// <example>91 80 6143 958-7</example>
    public EuropeanVehicleNumber Evn { get; private init; }
    
    /// <summary>
    /// The maximum allowed speed for the vehicle
    /// </summary>
    public float? MaxSpeed { get; private set; }
    
    internal Vehicle(VehicleId id, EuropeanVehicleNumber evn, float? maxSpeed) : base(id)
    {
        MaxSpeed = maxSpeed;
        Evn = evn;
    }

    /// <summary>
    /// Creates a single vehicle from a european vehicle number
    /// </summary>
    /// <param name="evn">European vehicle number of the vehicle</param>
    /// <exception cref="VehicleErrors.Locomotive.InvalidEvn">The evn is not of type locomotive</exception>
    /// <exception cref="VehicleErrors.PassengerCar.InvalidEvn">The evn is not of type passenger car</exception>
    /// <exception cref="VehicleErrors.NoSingleVehicle">The provided evn is part of a multiple unit and not a singular vehicle</exception>
    public static CanFail<Vehicle> Create(EuropeanVehicleNumber evn)
    {
        if (evn.IsPassengerCar)
        {
            var passengerCarResult = PassengerCar.Create(evn);
            if (passengerCarResult.HasFailed) return passengerCarResult.Errors;
            return passengerCarResult.Value;
        }
        
        if (evn.IsLocomotive)
        {
            var locomotiveResult = Locomotive.Create(evn);
            if (locomotiveResult.HasFailed) return locomotiveResult.Errors;
            return locomotiveResult.Value;
        }
        
        return VehicleErrors.NoSingleVehicle;
    }
}