using CleanDomainValidation.Domain;
using DBetter.TrainCompositions.Domain.CoachLayouts.ValueObjects;
using DBetter.TrainCompositions.Domain.Vehicles.ValueObjects;

namespace DBetter.TrainCompositions.Domain.Vehicles;

/// <summary>
/// Passenger car
/// </summary>
public class PassengerCar: Vehicle
{
    /// <summary>
    /// Indicates that the passenger car is a double-decker
    /// </summary>
    public bool IsDoubleDecker { get; private set; }
    
    /// <summary>
    /// Layout id of the coach, if specified
    /// </summary>
    public CoachLayoutId?  CoachLayoutId { get; private set; }
    
    internal PassengerCar(VehicleId id, EuropeanVehicleNumber evn, float? maxSpeed, bool isDoubleDecker, CoachLayoutId? coachLayoutId) : base(id, evn, maxSpeed)
    {
        IsDoubleDecker = isDoubleDecker;
        CoachLayoutId = coachLayoutId;
    }

    /// <summary>
    /// Creates a passenger car from an european vehicle number
    /// </summary>
    /// <param name="evn">European vehicle number of the car</param>
    /// <exception cref="VehicleErrors.PassengerCar.InvalidEvn">The evn is not of type passenger car</exception>
    internal new static CanFail<PassengerCar> Create(EuropeanVehicleNumber evn)
    {
        if (!evn.IsPassengerCar)
        {
            return VehicleErrors.PassengerCar.InvalidEvn;
        }

        var typeCode = evn.SeriesNumber.ToString();
        var isDoubleDecker = typeCode[1] == '6';
        var speedInformation = typeCode[2];
        float? maxSpeed = speedInformation switch
        {
            '0' => 120,
            '1' => 120,
            '2' => 120,
            '3' => 140,
            '4' => 140,
            '5' => 140,
            '6' => 140,
            '7' => 160,
            '8' => 160,
            '9' => null, //>160km/h but no specific number
            _ => null,
        };
        return new PassengerCar(VehicleId.CreateNew(), evn, maxSpeed, isDoubleDecker, null);
    }
}