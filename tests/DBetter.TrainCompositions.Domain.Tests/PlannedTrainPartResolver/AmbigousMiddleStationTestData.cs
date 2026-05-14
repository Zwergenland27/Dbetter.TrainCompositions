using System.Collections;
using DBetter.TrainCompositions.Domain.PlannedFormations.ValueObjects;
using DBetter.TrainCompositions.Domain.Shared.Stations;

namespace DBetter.TrainCompositions.Domain.Tests.PlannedTrainPartResolver;

public class AmbigousMiddleStationTestData: IEnumerable<object[]>
{
    private readonly List<object[]> _data = new List<object[]>
    {
        new object[] //First != Last -> Resulting in "middle" index 2
        {
            new List<(ExternalStationId, List<PlannedFormationId>)>
            {
                (PlannedTrainPartsResolverTests.Route[0].StationId, [PlannedTrainPartsResolverTests.FirstFormationId]),
                (PlannedTrainPartsResolverTests.Route[^2].StationId, [PlannedTrainPartsResolverTests.SecondFormationId]),
            },
            PlannedTrainPartsResolverTests.Route[2].StationId,
            PlannedTrainPartsResolverTests.Route[3].StationId
        },
        new object[] //First != Last && 2 == Last -> Resulting in "middle" index 1
        {
            new List<(ExternalStationId, List<PlannedFormationId>)>
            {
                (PlannedTrainPartsResolverTests.Route[0].StationId, [PlannedTrainPartsResolverTests.FirstFormationId]),
                (PlannedTrainPartsResolverTests.Route[2].StationId, [PlannedTrainPartsResolverTests.SecondFormationId]),
                (PlannedTrainPartsResolverTests.Route[^2].StationId, [PlannedTrainPartsResolverTests.SecondFormationId]),
            },
            PlannedTrainPartsResolverTests.Route[1].StationId,
            PlannedTrainPartsResolverTests.Route[2].StationId
        },
        new object[] //First != Last && 2 == First -> Resulting in "middle" index 3
        {
            new List<(ExternalStationId, List<PlannedFormationId>)>
            {
                (PlannedTrainPartsResolverTests.Route[0].StationId, [PlannedTrainPartsResolverTests.FirstFormationId]),
                (PlannedTrainPartsResolverTests.Route[2].StationId, [PlannedTrainPartsResolverTests.FirstFormationId]),
                (PlannedTrainPartsResolverTests.Route[^2].StationId, [PlannedTrainPartsResolverTests.SecondFormationId]),
            },
            PlannedTrainPartsResolverTests.Route[3].StationId,
            PlannedTrainPartsResolverTests.Route[4].StationId
        }
    };
    public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}