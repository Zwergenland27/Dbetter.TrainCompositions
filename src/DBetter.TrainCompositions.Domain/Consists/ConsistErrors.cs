using CleanDomainValidation.Domain;
using DBetter.TrainCompositions.Domain.Consists.ValueObjects;

namespace DBetter.TrainCompositions.Domain.Consists;

public static class ConsistErrors
{
    /// <summary>
    /// The coupled vehicles list is empty
    /// </summary>
    public static Error TooShort =>
        Error.Conflict("Consist.TooShort", "A consist must contain at least one vehicle");
    
    /// <summary>
    /// Errors of <see cref="ConsistId"/>
    /// </summary>
    public static class Id
    {
        /// <summary>
        /// The id is no valid guid
        /// </summary>
        public static Error Invalid => Error.Validation("Consist.Id.Invalid", "The provided id is no valid guid.");
    }
}