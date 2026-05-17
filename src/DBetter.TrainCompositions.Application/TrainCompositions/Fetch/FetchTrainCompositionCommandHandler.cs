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
    public override async Task<CanFail<List<PlannedTrainPart>>> Handle(FetchTrainCompositionCommand command, CancellationToken cancellationToken)
    {
        var trainRunResult = await trainRunProvider.GetRouteAsync(command.TrainRunId);
        if (trainRunResult.HasFailed) return trainRunResult.Errors;
        var trainRun = trainRunResult.Value;
        
        await unitOfWork.BeginTransaction(cancellationToken);
        
        var routeStops = trainRun.Stops
            .Select(s => new RouteStopSnapshot(s.RouteIndex, s.Id))
            .ToList();
        var plannedTrainPartsResolver = new PlannedTrainPartsResolver(routeStops);
        
        var plannedRouteFormationResolver = new PlannedRouteFormationResolver(new CoachLayoutResolver(coachLayoutRepository), new PlannedFormationResolver(plannedFormationRepository),  plannedFormationProvider);
        var plannedResult = await plannedRouteFormationResolver.GetPlannedAsync(trainRun, plannedTrainPartsResolver);
        if (plannedResult.HasFailed)
        {
            await unitOfWork.AbortAsync(cancellationToken);
            return plannedResult.Errors;
        }
        
        await unitOfWork.CommitAsync(cancellationToken);
        return plannedResult.Value.PlannedTrainParts;
    }
}