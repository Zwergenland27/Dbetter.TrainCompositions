using DBetter.TrainCompositions.Domain.PlannedFormations.ValueObjects;
using DBetter.TrainCompositions.Domain.Shared.Stations;

namespace DBetter.TrainCompositions.Domain.TrainCompositions.TrainParts;

/// <summary>
/// A planned train part of the composition
/// </summary>
public class PlannedTrainPart
{
    /// <summary>
    /// First station of this train part for this train run
    /// </summary>
    public ExternalStationId FromStation { get; private set; } 
    
    /// <summary>
    /// Last station of this train part for this train run
    /// </summary>
    public ExternalStationId ToStation { get; private set; }
    
    /// <summary>
    /// The planned formation for this train part
    /// </summary>
    public PlannedFormationId PlannedFormationId { get; private set; }
    
    internal PlannedTrainPart(ExternalStationId fromStation, ExternalStationId toStation, PlannedFormationId plannedFormationId)
    {
        FromStation = fromStation;
        ToStation = toStation;
        PlannedFormationId = plannedFormationId;
    }
}