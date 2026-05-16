using CleanDomainValidation.Domain;

namespace DBetter.TrainCompositions.Domain.Shared.TrainRuns;

/// <summary>
/// Errors of external train runs
/// </summary>
public class TrainRunErrors
{
    /// <summary>
    /// Placeholder error for upstream api errors
    /// </summary>
    public static Error Unknown =>
        Error.NotFound("TrainRun.Unknown", "Deserialising upstream errors is not supported yet.");
    
    /// <summary>
    /// Errors of <see cref="ExternalTrainRunId"/>
    /// </summary>
    public static class Id
    {
        /// <summary>
        /// The id is no valid guid
        /// </summary>
        public static Error Invalid => Error.Validation("TrainRun.Id.Invalid", "The provided id is no valid guid.");
    }
}