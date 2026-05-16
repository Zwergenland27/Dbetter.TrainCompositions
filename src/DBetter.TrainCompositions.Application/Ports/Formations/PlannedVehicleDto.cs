using DBetter.TrainCompositions.Domain.CoachLayouts.ValueObjects;

namespace DBetter.TrainCompositions.Application.Ports.Formations;

/// <summary>
/// Information about a vehicle of a planned formation
/// </summary>
public class PlannedVehicleDto
{
    /// <summary>
    /// Coach identifiers of the vehicle
    /// </summary>
    public required List<CoachLayoutIdentifier> CoachIdentifiers { get; init; }
}