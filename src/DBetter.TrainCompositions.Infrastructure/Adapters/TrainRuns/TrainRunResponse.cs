namespace DBetter.TrainCompositions.Infrastructure.Adapters.TrainRuns;

/// <summary>
/// Information about a train run
/// </summary>
public class TrainRunResponse
{
    /// <summary>
    /// Service number of the train, if available
    /// </summary>
    /// <example>1554 for ICE 1554</example>
    public int? ServiceNumber { get; set; }
    
    /// <summary>
    /// All Stops of the train run
    /// </summary>
    public required List<TrainRunStopResponse> Stops { get; set; }
}