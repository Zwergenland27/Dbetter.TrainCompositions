using System.Collections;
using DBetter.TrainCompositions.Domain.Shared.Stations;

namespace DBetter.TrainCompositions.Domain.Tests.PlannedTrainPartResolver;

public class MissingOriginStopTestData: IEnumerable<object[]>
{
    private readonly List<object[]> _data = new List<object[]>
    {
        new object[]{new List<ExternalStationId>()},
        new object[]{new List<ExternalStationId>{PlannedTrainPartsResolverTests.Route[^1].StationId}},
        new object[]{new List<ExternalStationId>{PlannedTrainPartsResolverTests.Route[4].StationId}},
        new object[]{new List<ExternalStationId>{PlannedTrainPartsResolverTests.Route[2].StationId, PlannedTrainPartsResolverTests.Route[3].StationId}},
    };
    public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

}