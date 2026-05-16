using CleanDomainValidation.Domain;

namespace DBetter.TrainCompositions.Domain.Shared.TrainRuns;

/// <summary>
/// Id of a train run. Original Source: DBetter
/// </summary>
/// <param name="Value"></param>
public record ExternalTrainRunId(Guid Value)
{
    public static CanFail<ExternalTrainRunId> Create(string value)
    {
        if (Guid.TryParse(value, out var guid))
        {
            return new ExternalTrainRunId(guid);
        }

        return TrainRunErrors.Id.Invalid;
    }
}