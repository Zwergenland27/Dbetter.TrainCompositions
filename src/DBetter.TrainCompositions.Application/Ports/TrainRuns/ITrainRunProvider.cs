using CleanDomainValidation.Domain;
using DBetter.TrainCompositions.Domain.Shared.TrainRuns;

namespace DBetter.TrainCompositions.Application.Ports.TrainRuns;

/// <summary>
/// Provides information about a train run of dbetter application
/// </summary>
public interface ITrainRunProvider
{
    /// <summary>
    /// Get route of the train run
    /// </summary>
    /// <param name="id">Id of the train run</param>
    /// <returns>Route information for the train run without any passenger related information</returns>
    Task<CanFail<TrainRunRouteDto>> GetRouteAsync(ExternalTrainRunId id);
}