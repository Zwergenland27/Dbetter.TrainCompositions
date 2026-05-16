using CleanMediator.Commands;
using DBetter.TrainCompositions.Domain.Shared.TrainRuns;
using DBetter.TrainCompositions.Domain.TrainCompositions.TrainParts;

namespace DBetter.TrainCompositions.Application.TrainCompositions.Fetch;

/// <summary>
/// Fetch the train composition for the specified train run
/// </summary>
public record FetchTrainCompositionCommand(ExternalTrainRunId TrainRunId) : ICommand<List<PlannedTrainPart>>;