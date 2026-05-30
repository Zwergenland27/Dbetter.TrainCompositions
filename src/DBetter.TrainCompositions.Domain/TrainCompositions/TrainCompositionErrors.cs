using CleanDomainValidation.Domain;

namespace DBetter.TrainCompositions.Domain.TrainCompositions;

/// <summary>
/// Errors of train composition
/// </summary>
public static class TrainCompositionErrors
{
    /// <summary>
    /// Will be returned if planned data for a train without a service number is requested
    /// </summary>
    public static Error PlannedNotAvailable => Error.Conflict("TrainComposition.PlannedNotAvailable",
        "Planned train compositions are not available for trains without a service number.");

    /// <summary>
    /// Will be returned when upstream api does not delivers data to identify planned train composition
    /// </summary>
    public static Error InsufficientData => Error.Conflict("TrainComposition.InsufficientData",
        "Planned train composition could not be fetched for the specified train run because no data is available");
}