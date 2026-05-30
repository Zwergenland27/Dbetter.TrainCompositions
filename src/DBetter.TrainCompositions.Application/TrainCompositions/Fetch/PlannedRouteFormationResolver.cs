using CleanDomainValidation.Domain;
using DBetter.TrainCompositions.Application.Ports.Formations;
using DBetter.TrainCompositions.Application.Ports.TrainRuns;
using DBetter.TrainCompositions.Domain.CoachLayouts;
using DBetter.TrainCompositions.Domain.PlannedFormations;
using DBetter.TrainCompositions.Domain.PlannedFormations.ValueObjects;
using DBetter.TrainCompositions.Domain.TrainCompositions;
using DBetter.TrainCompositions.Domain.TrainCompositions.TrainParts;

namespace DBetter.TrainCompositions.Application.TrainCompositions.Fetch;

public record PlannedRouteFormationResolverResult(List<PlannedTrainPart> PlannedTrainParts, List<CoachLayout> ResolvedCoachLayouts, List<PlannedFormation> ResolvedPlannedFormations);

public class PlannedRouteFormationResolver(
    ICoachLayoutResolver coachLayoutResolver,
    IPlannedFormationResolver plannedFormationResolver,
    IPlannedFormationProvider plannedFormationProvider)
{
    public async Task<CanFail<PlannedRouteFormationResolverResult>> GetPlannedAsync(TrainRunRouteDto trainRunRoute, IPlannedTrainPartsResolver plannedTrainPartsResolver)
    {
        if (trainRunRoute.ServiceNumber is null) return TrainCompositionErrors.PlannedNotAvailable;
        var serviceNumber = trainRunRoute.ServiceNumber!.Value;
        var stops = trainRunRoute.Stops;
        
        List<PlannedTrainPart>? result;
        while (!plannedTrainPartsResolver.Resolve(out var departureStationToScrape, out var arrivalStationToScrape, out result))
        {
            var originStop = stops.First(s => s.Id == departureStationToScrape);
            var destinationStop = stops.First(s => s.Id == arrivalStationToScrape);

            var observation = await GetPlannedFormationIdsAsync(serviceNumber, originStop, destinationStop);
            if (observation.HasFailed) return observation.Errors;
            
            plannedTrainPartsResolver.AddObservation(departureStationToScrape, observation.Value);
        }

        return new PlannedRouteFormationResolverResult(result,  coachLayoutResolver.AllKnownCoachLayouts.ToList(), plannedFormationResolver.AllKnownPlannedFormations.ToList());
    }
    
    private async Task<CanFail<List<PlannedFormationId>>> GetPlannedFormationIdsAsync(
        int serviceNumber, TrainRunStopDto departureStop, TrainRunStopDto destinationStop)
    {
        var plannedVehicleDtos = await plannedFormationProvider.GetForSectionAsync(serviceNumber,
            departureStop.EvaNumber, departureStop.PlannedDepartureTime!.Value,
            destinationStop.EvaNumber, destinationStop.PlannedArrivalTime!.Value);

        if (plannedVehicleDtos is null) return TrainCompositionErrors.InsufficientData;
        var coachLayoutIdentifier = plannedVehicleDtos
            .SelectMany(v => v.CoachIdentifiers)
            .ToList();
        
        var coachLayouts = await coachLayoutResolver.ResolveManyAsync(coachLayoutIdentifier);
        var coachLayoutDictionary = coachLayouts.ToDictionary(c => c.Identifier);
        
        var plannedFormationSnapshots = new List<PlannedFormationSnapshot>();
        foreach (var vehicle in plannedVehicleDtos)
        {
            var coachLayoutIds = vehicle.CoachIdentifiers
                .Select(c => coachLayoutDictionary[c].Id)
                .ToList();
            plannedFormationSnapshots.Add(new PlannedFormationSnapshot(coachLayoutIds));
        }
        
        var plannedFormations = await plannedFormationResolver.ResolveManyAsync(plannedFormationSnapshots);

        return plannedFormationSnapshots.Select(pfs => plannedFormations.First(pf => pf.Matches(pfs)).Id).ToList();
    }
}