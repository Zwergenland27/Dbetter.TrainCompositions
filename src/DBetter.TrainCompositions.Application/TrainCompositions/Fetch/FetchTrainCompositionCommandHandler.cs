using CleanDomainValidation.Domain;
using CleanMediator.Commands;
using DBetter.TrainCompositions.Application.Abstractions;
using DBetter.TrainCompositions.Application.Ports.Formations;
using DBetter.TrainCompositions.Application.Ports.TrainRuns;
using DBetter.TrainCompositions.Domain.CoachLayouts;
using DBetter.TrainCompositions.Domain.PlannedFormations;
using DBetter.TrainCompositions.Domain.PlannedFormations.ValueObjects;
using DBetter.TrainCompositions.Domain.Shared.TrainRuns;
using DBetter.TrainCompositions.Domain.TrainCompositions;
using DBetter.TrainCompositions.Domain.TrainCompositions.TrainParts;

namespace DBetter.TrainCompositions.Application.TrainCompositions.Fetch;

public class FetchTrainCompositionCommandHandler(
    IUnitOfWork unitOfWork,
    ICoachLayoutRepository coachLayoutRepository,
    IPlannedFormationRepository plannedFormationRepository,
    IPlannedFormationProvider plannedFormationProvider,
    ITrainRunProvider trainRunProvider): CommandHandlerBase<FetchTrainCompositionCommand, List<PlannedTrainPart>>
{
    private record PlannedFormationResult(List<PlannedTrainPart> PlannedTrainParts, List<CoachLayout> ResolvedCoachLayouts, List<PlannedFormation> ResolvedPlannedFormations);
    
    public override async Task<CanFail<List<PlannedTrainPart>>> Handle(FetchTrainCompositionCommand command, CancellationToken cancellationToken)
    {
        var trainRunResult = await trainRunProvider.GetRouteAsync(command.TrainRunId);
        if (trainRunResult.HasFailed) return trainRunResult.Errors;
        var trainRun = trainRunResult.Value;
        
        await unitOfWork.BeginTransaction(cancellationToken);
        
        var plannedResult = await GetPlannedAsync(trainRun);
        if (plannedResult.HasFailed)
        {
            await unitOfWork.AbortAsync(cancellationToken);
            return plannedResult.Errors;
        }
        
        await unitOfWork.CommitAsync(cancellationToken);
        return plannedResult.Value.PlannedTrainParts;
    }

    private async Task<CanFail<PlannedFormationResult>> GetPlannedAsync(TrainRunRouteDto trainRunRoute)
    {
        if (trainRunRoute.ServiceNumber is null) return TrainCompositionErrors.PlannedNotAvailable;
        var serviceNumber = trainRunRoute.ServiceNumber!.Value;
        var stops = trainRunRoute.Stops;
        var knownCoachLayouts =new List<CoachLayout>();
        var knownPlannedFormations = new List<PlannedFormation>();
        
        var routeStops = stops
            .Select(s => new RouteStopSnapshot(s.RouteIndex, s.Id))
            .ToList();
        
        var plannedTrainPartsResolver = new PlannedTrainPartsResolver(routeStops);
        List<PlannedTrainPart>? result;
        while (!plannedTrainPartsResolver.Resolve(out var departureStationToScrape, out var arrivalStationToScrape, out result))
        {
            var originStop = stops.First(s => s.Id == departureStationToScrape);
            var destinationStop = stops.First(s => s.Id == arrivalStationToScrape);

            var observation = await GetPlannedFormationIdsAsync(knownCoachLayouts, knownPlannedFormations, serviceNumber, originStop, destinationStop);
            if(observation.HasFailed) throw new NotImplementedException();
            
            plannedTrainPartsResolver.AddObservation(departureStationToScrape, observation.Value);
        }

        return new PlannedFormationResult(result,  knownCoachLayouts, knownPlannedFormations);
    }
    
    private async Task<CanFail<List<PlannedFormationId>>> GetPlannedFormationIdsAsync(
        List<CoachLayout> knownCoachLayouts, List<PlannedFormation> knownPlannedFormations,
        int serviceNumber, TrainRunStopDto departureStop, TrainRunStopDto destinationStop)
    {
        var plannedVehicleDtos = await plannedFormationProvider.GetForSectionAsync(serviceNumber,
            departureStop.EvaNumber, departureStop.PlannedDepartureTime!.Value,
            destinationStop.EvaNumber, destinationStop.PlannedArrivalTime!.Value);

        if (plannedVehicleDtos is null) throw new NotImplementedException();
        var coachLayoutIdentifier = plannedVehicleDtos
            .SelectMany(v => v.CoachIdentifiers)
            .ToList();
        
        var coachLayouts = await new CoachLayoutResolver(coachLayoutRepository, knownCoachLayouts)
            .ResolveMany(coachLayoutIdentifier);
        var coachLayoutDictionary = coachLayouts.ToDictionary(c => c.Identifier);
        
        var plannedFormationSnapshots = new List<PlannedFormationSnapshot>();
        foreach (var vehicle in plannedVehicleDtos)
        {
            var coachLayoutIds = vehicle.CoachIdentifiers
                .Select(c => coachLayoutDictionary[c].Id)
                .ToList();
            plannedFormationSnapshots.Add(new PlannedFormationSnapshot(coachLayoutIds));
        }
        
        var plannedFormations = await new PlannedFormationResolver(plannedFormationRepository, knownPlannedFormations)
            .ResolveManyAsync(plannedFormationSnapshots);

        return plannedFormationSnapshots.Select(pfs => plannedFormations.First(pf => pf.Matches(pfs)).Id).ToList();
    }
}