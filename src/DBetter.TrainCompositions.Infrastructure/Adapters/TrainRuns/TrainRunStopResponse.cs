using DBetter.TrainCompositions.Infrastructure.DBetter;
using DBetter.TrainCompositions.Infrastructure.TrainRuns;

namespace DBetter.TrainCompositions.Infrastructure.Adapters.TrainRuns;

/// <summary>
/// Stop of a <see cref="TrainRunResponse"/>
/// </summary>
public class TrainRunStopResponse
{
    /// <summary>
    /// Internal id of the station
    /// </summary>
    public required string Id { get; init; }
    
    /// <summary>
    /// Eva number of the station
    /// </summary>
    public required string EvaNumber { get; init; }

    /// <summary>
    /// Departure time
    /// </summary>
    public required TravelTimeResponse? DepartureTime { get; init; }
    
    /// <summary>
    /// Arrival time
    /// </summary>
    public required TravelTimeResponse? ArrivalTime { get; init; }
    
    /// <summary>
    /// Platform
    /// </summary>
    public required PlatformResponse? Platform { get; init; }
    
    /// <summary>
    /// Stop index of the full train run
    /// </summary>
    public required int RouteIndex { get; init; }
}