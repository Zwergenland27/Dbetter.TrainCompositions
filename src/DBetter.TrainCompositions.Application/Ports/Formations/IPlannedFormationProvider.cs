using DBetter.TrainCompositions.Application.TrainCompositions;

namespace DBetter.TrainCompositions.Application.Ports.Formations;

/// <summary>
/// Provider to gain the planned formation of a train run
/// </summary>
public interface IPlannedFormationProvider
{
    /// <summary>
    /// Retrieves the planned formation for the section between <paramref name="originStationEva"/> and <paramref name="destinationStationEva"/>
    /// </summary>
    /// <param name="serviceNumber">Service number of the train</param>
    /// <param name="originStationEva">Eva number of the origin station of the section</param>
    /// <param name="departureTime">Planned departure time at the origin station</param>
    /// <param name="destinationStationEva">Eva number of the destination station of the section</param>
    /// <param name="arrivalTime">Planned arrival time at the destination station</param>
    /// <returns>The planned vehicles if any found, otherwise null</returns>
    Task<List<PlannedVehicleDto>?> GetForSectionAsync(int serviceNumber, string originStationEva, DateTime departureTime, string destinationStationEva, DateTime arrivalTime);
}