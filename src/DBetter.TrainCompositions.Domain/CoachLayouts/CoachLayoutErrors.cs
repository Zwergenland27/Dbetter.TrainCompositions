using CleanDomainValidation.Domain;
using DBetter.TrainCompositions.Domain.CoachLayouts.ValueObjects;

namespace DBetter.TrainCompositions.Domain.CoachLayouts;

/// <summary>
/// Errors of <see cref="CoachLayout"/>
/// </summary>
public static class CoachLayoutErrors
{
    /// <summary>
    /// Errors of <see cref="CoachLayoutId"/>
    /// </summary>
    public static class Id
    {
        /// <summary>
        /// The id is no valid guid
        /// </summary>
        public static Error Invalid => Error.Validation("CoachLayout.Id.Invalid", $"The provided id is no valid guid.");
    }
}