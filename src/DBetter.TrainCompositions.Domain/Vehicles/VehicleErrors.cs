using CleanDomainValidation.Domain;
using DBetter.TrainCompositions.Domain.Vehicles.ValueObjects;

namespace DBetter.TrainCompositions.Domain.Vehicles;

public static class VehicleErrors
{
    /// <summary>
    /// The provided evn is part of a multiple unit and not a singular vehicle
    /// </summary>
    public static Error NoSingleVehicle => Error.Conflict("Vehicle.NoSingleVehicle",
        "The provided evn associates to a multiple unit and cannot be mapped to a singular vehicle.");
    
    /// <summary>
    /// Errors of the <see cref="EuropeanVehicleNumber"/>
    /// </summary>
    public static class Evn
    {
        /// <summary>
        /// The evn is an empty string
        /// </summary>
        public static Error Empty =>
            Error.Validation("Vehicle.Evn.Empty", "The european vehicle number cannot be empty.");

        /// <summary>
        /// The length of the digit only evn does not match the specification of 12
        /// </summary>
        public static Error InvalidLength => Error.Validation("Vehicle.Evn.InvalidLength",
            "The european vehicle number must contain exactly 12 digits.");

        /// <summary>
        /// The check digit of the provided evn does not match the calculated one
        /// </summary>
        public static Error InvalidCheckDigit => Error.Validation("Vehicle.Evn.InvalidCheckDigit",
            "The check digit of the european vehicle number is incorrect.");
    }
    
    /// <summary>
    /// Errors of <see cref="VehicleId"/>
    /// </summary>
    public static class Id
    {
        /// <summary>
        /// The id is no valid guid
        /// </summary>
        public static Error Invalid => Error.Validation("Vehicle.Id.Invalid", "The provided id is no valid guid.");
    }

    /// <summary>
    /// Errors of <see cref="Vehicles.Locomotive"/>
    /// </summary>
    public static class Locomotive
    {
        /// <summary>
        /// The evn is not of type locomotive
        /// </summary>
        public static Error InvalidEvn => Error.Conflict("Vehicle.Locomotive.InvalidEvn",
            "The provided evn does not declare a locomotive");
    }
    
    /// <summary>
    /// Errors of <see cref="Vehicles.PassengerCar"/>
    /// </summary>
    public static class PassengerCar
    {
        /// <summary>
        /// The evn is not of type passenger car
        /// </summary>
        public static Error InvalidEvn => Error.Conflict("Vehicle.PassengerCar.InvalidEvn",
            "The provided evn does not declare a passenger car");
    }
}