using DBetter.TrainCompositions.Domain.Shared.Stations;

namespace DBetter.TrainCompositions.Application.Ports.TrainRuns;

/// <summary>
/// Stop of a <see cref="TrainRunRouteDto"/>
/// </summary>
public class TrainRunStopDto
{
    /// <summary>
    /// Internal id of the station
    /// </summary>
    public required ExternalStationId Id { get; init; }
    
    /// <summary>
    /// Eva number of the station
    /// </summary>
    public required string EvaNumber { get; init; }

    /// <summary>
    /// Departure time
    /// </summary>
    public required DateTime? PlannedDepartureTime { get; init; }
    
    /// <summary>
    /// Arrival time
    /// </summary>
    public required DateTime? PlannedArrivalTime { get; init; }
    
    /// <summary>
    /// Stop index of the full train run
    /// </summary>
    public required int RouteIndex { get; init; }
}