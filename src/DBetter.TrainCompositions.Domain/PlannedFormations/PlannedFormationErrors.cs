using CleanDomainValidation.Domain;
using DBetter.TrainCompositions.Domain.PlannedFormations.ValueObjects;

namespace DBetter.TrainCompositions.Domain.PlannedFormations;

/// <summary>
/// Errors of <see cref="PlannedFormation"/>
/// </summary>
public static class PlannedFormationErrors
{
    /// <summary>
    /// The coach list is empty
    /// </summary>
    public static Error TooShort =>
        Error.Conflict("PlannedFormation.TooShort", "A planned formation mus contain at least one coach");
    
    /// <summary>
    /// Errors of <see cref="PlannedFormationId"/>
    /// </summary>
    public static class Id
    {
        /// <summary>
        /// The id is no valid guid
        /// </summary>
        public static Error Invalid => Error.Validation("PlannedFormation.Id.Invalid", "The provided id is no valid guid.");
    }
}