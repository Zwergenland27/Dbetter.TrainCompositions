namespace DBetter.TrainCompositions.Infrastructure.DBetter;

/// <summary>
/// Departure / arrival time
/// </summary>
public class TravelTimeResponse
{
    /// <summary>
    /// Planned time
    /// </summary>
    public required DateTime Planned  { get; set; }
    
    /// <summary>
    /// Real time
    /// </summary>
    public required DateTime? Real { get; set; }
}