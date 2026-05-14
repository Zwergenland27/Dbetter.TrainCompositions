using CleanDomainValidation.Domain;

namespace DBetter.TrainCompositions.Domain.Shared.Stations;

/// <summary>
/// Id of a Station. Original Source: DBetter
/// </summary>
/// <param name="Value"></param>
public record ExternalStationId(Guid Value)
{
    public static CanFail<ExternalStationId> Create(string value)
    {
        if (Guid.TryParse(value, out var guid))
        {
            return new ExternalStationId(guid);
        }

        return StationErrors.Id.Invalid;
    }
}