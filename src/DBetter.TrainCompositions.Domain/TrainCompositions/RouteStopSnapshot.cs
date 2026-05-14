using DBetter.TrainCompositions.Domain.Shared.Stations;

namespace DBetter.TrainCompositions.Domain.TrainCompositions;

/// <summary>
/// Snapshot of a stop of a train run route
/// </summary>
/// <param name="RouteIndex">Index of the stop of the current route</param>
/// <param name="StationId">Id of the station</param>
public record RouteStopSnapshot(int RouteIndex, ExternalStationId StationId);