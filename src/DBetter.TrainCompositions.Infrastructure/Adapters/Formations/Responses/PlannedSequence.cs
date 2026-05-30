namespace DBetter.TrainCompositions.Infrastructure.TrainRuns;

public class PlannedSequence
{
    public required string Type { get; set; }
    
    public required string Version { get; set; }
    
    public required Zugfahrt Zugfahrt { get; set; }
}