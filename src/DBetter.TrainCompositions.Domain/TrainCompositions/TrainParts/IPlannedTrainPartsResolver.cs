using System.Diagnostics.CodeAnalysis;
using DBetter.TrainCompositions.Domain.PlannedFormations.ValueObjects;
using DBetter.TrainCompositions.Domain.Shared.Stations;

namespace DBetter.TrainCompositions.Domain.TrainCompositions.TrainParts;

/// <summary>
/// Resolves the planned train parts for 
/// </summary>
public interface IPlannedTrainPartsResolver
{
    /// <summary>
    /// Add observation for the specified section
    /// </summary>
    /// <param name="departureStation">The departure station of the observed section</param>
    /// <param name="observedFormations">The observed formation for the section</param>
    void AddObservation(ExternalStationId departureStation, List<PlannedFormationId> observedFormations);

    /// <summary>
    /// Tries to build the train parts of the journey
    /// </summary>
    /// <param name="departureStationToScrape">The departure station that should be scraped to capture further information</param>
    /// <param name="arrivalStationToScrape">The arrival station that should be scraped to capture further information</param>
    /// <param name="plannedTrainParts">All train parts, when unambiguous configuration has been found</param>
    /// <returns>True, if an unambiguous configuration has been found</returns>
    bool Resolve(
        [MaybeNullWhen(true)] out ExternalStationId departureStationToScrape,
        [MaybeNullWhen(true)] out ExternalStationId arrivalStationToScrape,
        [MaybeNullWhen(false)] out List<PlannedTrainPart> plannedTrainParts);
}