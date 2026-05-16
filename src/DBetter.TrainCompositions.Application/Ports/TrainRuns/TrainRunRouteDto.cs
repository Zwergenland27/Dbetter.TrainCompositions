namespace DBetter.TrainCompositions.Application.Ports.TrainRuns;

/// <summary>
/// Information about a train run
/// </summary>
public class TrainRunRouteDto
{
    /// <summary>
    /// Service number of the train, if available
    /// </summary>
    /// <example>1554 for ICE 1554</example>
    public required int? ServiceNumber { get; set; }
    
    /// <summary>
    /// All Stops of the train run
    /// </summary>
    public required List<TrainRunStopDto> Stops { get; set; }
}