using CleanDomainValidation.Domain;

namespace DBetter.TrainCompositions.Domain.Shared.Stations;

/// <summary>
/// Errors of external Station
/// </summary>
public static class StationErrors
{
    /// <summary>
    /// Errors of <see cref="ExternalStationId"/>
    /// </summary>
    public static class Id
    {
        /// <summary>
        /// The id is no valid guid
        /// </summary>
        public static Error Invalid => Error.Validation("PlannedFormation.Id.Invalid", "The provided id is no valid guid.");
    }
}