using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using DBetter.TrainCompositions.Domain.PlannedFormations.ValueObjects;
using DBetter.TrainCompositions.Domain.Shared.Stations;

namespace DBetter.TrainCompositions.Domain.TrainCompositions.TrainParts;

/// <inheritdoc/>
public class PlannedTrainPartsResolver: IPlannedTrainPartsResolver
{
    private record Interval(RouteStopSnapshot Left, RouteStopSnapshot Right);

    private record UnambiguousFormationId(PlannedFormationId FormationId, int Index);
    
    private readonly ImmutableList<RouteStopSnapshot> _orderedRoute;
    private readonly Dictionary<ExternalStationId, List<UnambiguousFormationId>> _observations;

    private bool _observationAddedSinceLastResolve = true;

    public PlannedTrainPartsResolver(List<RouteStopSnapshot> route) 
    {
        _orderedRoute = route.OrderBy(r => r.RouteIndex).ToImmutableList();
        _observations = [];
    }

    /// <inheritdoc/>
    public void AddObservation(ExternalStationId departureStation, List<PlannedFormationId> observedFormations)
    {
        _observationAddedSinceLastResolve = true;
        var unambiguousFormationIds = new List<UnambiguousFormationId>();
        foreach (var observedFormationId in observedFormations)
        {
            var existing = unambiguousFormationIds.FirstOrDefault(unambiguousCoachLayoutId =>
                observedFormationId == unambiguousCoachLayoutId.FormationId);
            if (existing is null)
            {
                unambiguousFormationIds.Add(new UnambiguousFormationId(observedFormationId, 0));
            }
            else {
                unambiguousFormationIds.Add(new UnambiguousFormationId(observedFormationId, existing.Index + 1));
            }
        }
        
        _observations[departureStation] = unambiguousFormationIds;
    }

    /// <inheritdoc/>
    public bool Resolve(
        [MaybeNullWhen(true)] out ExternalStationId departureStationToScrape,
        [MaybeNullWhen(true)] out ExternalStationId arrivalStationToScrape,
        [MaybeNullWhen(false)] out List<PlannedTrainPart> plannedTrainParts)

    {
        if (!_observationAddedSinceLastResolve)
            throw new InvalidOperationException("Resolve was called without a new observation since the last call.");
    
        _observationAddedSinceLastResolve = false;
        var orderedRoute = _orderedRoute.OrderBy(r => r.RouteIndex).ToList();
        var firstDepartureStop = orderedRoute.First();
        var lastDepartureStop = orderedRoute[^2];

        //plannedSnapshots must contain first station
        if (!_observations.ContainsKey(firstDepartureStop.StationId))
        {
            departureStationToScrape = firstDepartureStop.StationId;
            arrivalStationToScrape = orderedRoute[1].StationId;
            plannedTrainParts = null;
            return false;
        }

        //plannedSnapshots must contain last station
        if (!_observations.ContainsKey(lastDepartureStop.StationId))
        {
            departureStationToScrape = lastDepartureStop.StationId;
            arrivalStationToScrape = orderedRoute[^1].StationId;
            plannedTrainParts = null;
            return false;
        }

        var openIntervals = BuildInitialIntervals();
        while (openIntervals.Any())
        {
            var interval = openIntervals.Dequeue();
            var leftLayout = _observations[interval.Left.StationId];
            var rightLayout = _observations[interval.Right.StationId];
            if (IsTrainPartsEqual(leftLayout, rightLayout))
            {
                continue;
            }
            
            var pivotStation = CalculatePivotStation(interval.Left, interval.Right);
            if (pivotStation is null)
            {
                continue;
            }

            if (!_observations.ContainsKey(pivotStation.StationId))
            {
                departureStationToScrape = pivotStation.StationId;
                arrivalStationToScrape = _orderedRoute[pivotStation.RouteIndex + 1].StationId;
                plannedTrainParts = null;
                return false;
            }
            
            openIntervals.Enqueue(interval with { Right = pivotStation });
            openIntervals.Enqueue(interval with { Left = pivotStation });
        }

        departureStationToScrape = null;
        arrivalStationToScrape = null;
        plannedTrainParts = ExtractTrainParts();
        return true;
    }

    private List<PlannedTrainPart> ExtractTrainParts()
    {
        var trainParts = new List<PlannedTrainPart>();

        var unambiguousLayoutIds = _observations
            .SelectMany(o => o.Value)
            .DistinctBy(s => new { LayoutId = s.FormationId, s.Index});

        foreach (var layoutId in unambiguousLayoutIds)
        {
            var observedStationIds = _observations
                .Where(o => o.Value.Contains(layoutId))
                .Select(o => o.Key);
            
            var routeStops = _orderedRoute
                .Where(s => observedStationIds.Contains(s.StationId))
                .ToList();
            
            var firstStop = routeStops.MinBy(s => s.RouteIndex)!;
            var lastStopWithDeparture = routeStops.MaxBy(s => s.RouteIndex)!;
            var lastStop = lastStopWithDeparture;
            if (lastStopWithDeparture.RouteIndex != _orderedRoute.Last().RouteIndex)
            {
                lastStop = _orderedRoute[lastStopWithDeparture.RouteIndex + 1];
            }
            
            trainParts.Add(new PlannedTrainPart(
                firstStop.StationId,
                lastStop.StationId,
                layoutId.FormationId));
        }
        
        return trainParts;
    }

    private Queue<Interval> BuildInitialIntervals()
    {
        var observedStops = _orderedRoute
            .Where(s => _observations.ContainsKey(s.StationId))
            .OrderBy(r => r.RouteIndex)
            .ToList();

        var queue = new Queue<Interval>();
        for (int i = 0; i < observedStops.Count - 1; i++)
        {
            queue.Enqueue(new Interval(observedStops[i], observedStops[i+1]));
        }

        return queue;
    }

    private RouteStopSnapshot? CalculatePivotStation(RouteStopSnapshot left, RouteStopSnapshot right)
    {
        var candidates = _orderedRoute
            .Where(s => s.RouteIndex > left.RouteIndex && s.RouteIndex < right.RouteIndex &&
                        !_observations.ContainsKey(s.StationId))
            .OrderBy(s => s.RouteIndex)
            .ToList();

        if (!candidates.Any()) return null;

        var midIndex = (left.RouteIndex + right.RouteIndex) / 2;
        return candidates.MinBy(s => Math.Abs(s.RouteIndex - midIndex));
    }

    private bool IsTrainPartsEqual(List<UnambiguousFormationId> a, List<UnambiguousFormationId> b)
    {
        var idsA = a.Select(v => $"{v.FormationId.Value}-{v.Index}").ToHashSet();
        var idsB = b.Select(v => $"{v.FormationId.Value}-{v.Index}").ToHashSet();
        
        return idsB.SetEquals(idsA);
    }
}