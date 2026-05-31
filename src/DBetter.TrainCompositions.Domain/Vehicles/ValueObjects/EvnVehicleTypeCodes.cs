namespace DBetter.TrainCompositions.Domain.Vehicles.ValueObjects;

/// <summary>
/// Codes to identify the vehicle typ of a european vehicle number according to https://de.wikipedia.org/wiki/Code_f%C3%BCr_das_Austauschverfahren
/// </summary>
public static class EvnVehicleTypeCodes
{
    /*
     * Passenger cars
     */
    // ── Not internationally usable under RIC ────────────────────────────────

    /// <summary>00 – Vehicle decommissioned</summary>
    public const int Decommissioned = 00;

    /// <summary>50 – General domestic passenger coaches</summary>
    public const int DomesticPassengerCoach = 50;

    /// <summary>55 – Private domestic passenger coaches</summary>
    public const int PrivateDomesticPassengerCoach = 55;

    /// <summary>60 – Railway service vehicles of passenger coach design</summary>
    public const int RailwayServiceCoach = 60;

    /// <summary>65 – Car-carrier wagons of freight wagon design</summary>
    public const int CarCarrierWagon = 65;

    /// <summary>70 – Pressure-tight and air-conditioned passenger coaches</summary>
    public const int PressureTightAirConditionedCoach = 70;

    /// <summary>75 – Passenger coaches of private operators</summary>
    public const int PrivateOperatorPassengerCoach = 75;

    // ── Internationally usable under RIC ────────────────────────────────────

    /// <summary>51 – Non-air-conditioned passenger coaches, fixed gauge</summary>
    public const int NonAirConditionedFixedGaugeCoach = 51;

    /// <summary>52 – Non-air-conditioned passenger coaches, variable gauge (1435/1520 mm)</summary>
    public const int NonAirConditionedVariableGauge1520Coach = 52;

    /// <summary>54 – Non-air-conditioned passenger coaches, variable gauge (1435/1668 mm)</summary>
    public const int NonAirConditionedVariableGauge1668Coach = 54;

    /// <summary>56 – Non-air-conditioned private passenger coaches, fixed gauge</summary>
    public const int PrivateNonAirConditionedFixedGaugeCoach = 56;

    /// <summary>61 – Air-conditioned passenger coaches, fixed gauge</summary>
    public const int AirConditionedFixedGaugeCoach = 61;

    /// <summary>62 – Air-conditioned passenger coaches, variable gauge (1435/1520 mm)</summary>
    public const int AirConditionedVariableGauge1520Coach = 62;

    /// <summary>63 – Railway service vehicles of passenger coach design (RIC)</summary>
    public const int RicRailwayServiceCoach = 63;

    /// <summary>64 – Air-conditioned passenger coaches, variable gauge (1435/1668 mm)</summary>
    public const int AirConditionedVariableGauge1668Coach = 64;

    /// <summary>66 – Air-conditioned private passenger coaches, variable gauge (1435/1668 mm)</summary>
    public const int PrivateAirConditionedVariableGauge1668Coach = 66;

    /// <summary>71 – Sleeping cars</summary>
    public const int SleepingCar = 71;

    /// <summary>73 – Air-conditioned and pressure-resistant coaches, fixed gauge</summary>
    public const int AirConditionedPressureResistantFixedGaugeCoach = 73;

    /// <summary>
    /// All kinds of passenger cars
    /// </summary>
    public static int[] PassengerCars =>
    [
        DomesticPassengerCoach,
        PrivateDomesticPassengerCoach,
        RailwayServiceCoach,
        CarCarrierWagon,
        PressureTightAirConditionedCoach,
        PrivateOperatorPassengerCoach,
        NonAirConditionedFixedGaugeCoach,
        NonAirConditionedVariableGauge1520Coach,
        NonAirConditionedVariableGauge1668Coach,
        PrivateNonAirConditionedFixedGaugeCoach,
        AirConditionedFixedGaugeCoach,
        AirConditionedVariableGauge1520Coach,
        RicRailwayServiceCoach,
        AirConditionedVariableGauge1668Coach,
        PrivateAirConditionedVariableGauge1668Coach,
        SleepingCar,
        AirConditionedPressureResistantFixedGaugeCoach
    ];
    
    /*
     * Locomotives
     */
    
    /// <summary>90 – Miscellaneous vehicles</summary>
    public const int Miscellaneous = 90;

    /// <summary>91 – Electric locomotives</summary>
    public const int ElectricLocomotive = 91;

    /// <summary>92 – Diesel locomotives</summary>
    public const int DieselLocomotive = 92;

    /// <summary>93 – Electric high-speed multiple units (power cars and trailer cars)</summary>
    public const int ElectricHighSpeedMultipleUnit = 93;

    /// <summary>94 – Electric multiple units, non-high-speed (power cars and trailer cars)</summary>
    public const int ElectricMultipleUnit = 94;

    /// <summary>95 – Diesel multiple units (power cars and trailer cars)</summary>
    public const int DieselMultipleUnit = 95;

    /// <summary>96 – Special trailer cars</summary>
    public const int SpecialTrailerCar = 96;

    /// <summary>97 – Electric shunting locomotives</summary>
    public const int ElectricShuntingLocomotive = 97;

    /// <summary>98 – Diesel shunting locomotives</summary>
    public const int DieselShuntingLocomotive = 98;

    /// <summary>99 – Special vehicles</summary>
    public const int SpecialVehicle = 99;

    /// <summary>
    /// All kinds of locomotives
    /// </summary>
    public static int[] Locomotives =>
    [
        Miscellaneous,
        ElectricLocomotive,
        DieselLocomotive,
        ElectricShuntingLocomotive,
        DieselShuntingLocomotive
    ];

    /// <summary>
    /// All kinds of multiple units
    /// </summary>
    public static int[] MultipleUnits =>
    [
        ElectricHighSpeedMultipleUnit,
        ElectricMultipleUnit,
        DieselMultipleUnit
    ];
}