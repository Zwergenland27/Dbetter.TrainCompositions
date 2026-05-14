using System.Collections;
using DBetter.TrainCompositions.Domain.PlannedFormations.ValueObjects;
using DBetter.TrainCompositions.Domain.Shared.Stations;
using DBetter.TrainCompositions.Domain.TrainCompositions.TrainParts;

namespace DBetter.TrainCompositions.Domain.Tests.PlannedTrainPartResolver;

public class ResolvedAmbigousMiddleStationTestData: IEnumerable<object[]>
{
    private readonly List<object[]> _data = new List<object[]>
    {
        new object[] //Layout 1: Route[0] - Route[2], Layout 2: Route[2] - Route[End]
        {
            new List<(ExternalStationId, List<PlannedFormationId>)>
            {
                (PlannedTrainPartsResolverTests.Route[0].StationId, [PlannedTrainPartsResolverTests.FirstFormationId]),
                (PlannedTrainPartsResolverTests.Route[1].StationId, [PlannedTrainPartsResolverTests.FirstFormationId]),
                (PlannedTrainPartsResolverTests.Route[2].StationId, [PlannedTrainPartsResolverTests.SecondFormationId]),
                (PlannedTrainPartsResolverTests.Route[^2].StationId, [PlannedTrainPartsResolverTests.SecondFormationId]),
            },
            new List<PlannedTrainPart>
            {
                new (PlannedTrainPartsResolverTests.Route[0].StationId, PlannedTrainPartsResolverTests.Route[2].StationId, PlannedTrainPartsResolverTests.FirstFormationId),
                new (PlannedTrainPartsResolverTests.Route[2].StationId, PlannedTrainPartsResolverTests.Route[^1].StationId, PlannedTrainPartsResolverTests.SecondFormationId),
            }
        },
        new object[] //Layout 1: Route[0] - Route[1], Layout 2: Route[0] - Route[End]
        {
            new List<(ExternalStationId, List<PlannedFormationId>)>
            {
                (PlannedTrainPartsResolverTests.Route[0].StationId, [PlannedTrainPartsResolverTests.SecondFormationId, PlannedTrainPartsResolverTests.FirstFormationId]),
                (PlannedTrainPartsResolverTests.Route[1].StationId, [PlannedTrainPartsResolverTests.SecondFormationId, PlannedTrainPartsResolverTests.FirstFormationId]),
                (PlannedTrainPartsResolverTests.Route[2].StationId, [PlannedTrainPartsResolverTests.SecondFormationId]),
                (PlannedTrainPartsResolverTests.Route[^2].StationId, [PlannedTrainPartsResolverTests.SecondFormationId]),
            },
            new List<PlannedTrainPart>
            {
                new (PlannedTrainPartsResolverTests.Route[0].StationId, PlannedTrainPartsResolverTests.Route[2].StationId, PlannedTrainPartsResolverTests.FirstFormationId),
                new (PlannedTrainPartsResolverTests.Route[0].StationId, PlannedTrainPartsResolverTests.Route[^1].StationId, PlannedTrainPartsResolverTests.SecondFormationId),
            }
        },
        new object[] //Layout 1: Route[0] - Route[1], Layout 2: Route[0] - Route[End]
        {
            new List<(ExternalStationId, List<PlannedFormationId>)>
            {
                (PlannedTrainPartsResolverTests.Route[0].StationId, [PlannedTrainPartsResolverTests.SecondFormationId, PlannedTrainPartsResolverTests.FirstFormationId]),
                (PlannedTrainPartsResolverTests.Route[1].StationId, [PlannedTrainPartsResolverTests.FirstFormationId, PlannedTrainPartsResolverTests.SecondFormationId]),
                (PlannedTrainPartsResolverTests.Route[2].StationId, [PlannedTrainPartsResolverTests.SecondFormationId]),
                (PlannedTrainPartsResolverTests.Route[^2].StationId, [PlannedTrainPartsResolverTests.SecondFormationId]),
            },
            new List<PlannedTrainPart>
            {
                new (PlannedTrainPartsResolverTests.Route[0].StationId, PlannedTrainPartsResolverTests.Route[2].StationId, PlannedTrainPartsResolverTests.FirstFormationId),
                new (PlannedTrainPartsResolverTests.Route[0].StationId, PlannedTrainPartsResolverTests.Route[^1].StationId, PlannedTrainPartsResolverTests.SecondFormationId),
            }
        },
        new object[] //Layout 1: Route[0] - Route[End], Layout 2: Route[3] - Route[End]
        {
            new List<(ExternalStationId, List<PlannedFormationId>)>
            {
                (PlannedTrainPartsResolverTests.Route[0].StationId, [PlannedTrainPartsResolverTests.FirstFormationId]),
                (PlannedTrainPartsResolverTests.Route[2].StationId, [PlannedTrainPartsResolverTests.FirstFormationId]),
                (PlannedTrainPartsResolverTests.Route[3].StationId, [PlannedTrainPartsResolverTests.FirstFormationId, PlannedTrainPartsResolverTests.SecondFormationId]),
                (PlannedTrainPartsResolverTests.Route[^2].StationId, [PlannedTrainPartsResolverTests.FirstFormationId, PlannedTrainPartsResolverTests.SecondFormationId]),
            },
            new List<PlannedTrainPart>
            {
                new (PlannedTrainPartsResolverTests.Route[0].StationId, PlannedTrainPartsResolverTests.Route[^1].StationId, PlannedTrainPartsResolverTests.FirstFormationId),
                new (PlannedTrainPartsResolverTests.Route[3].StationId, PlannedTrainPartsResolverTests.Route[^1].StationId, PlannedTrainPartsResolverTests.SecondFormationId),
            }
        },
        new object[] //Layout 1 + Layout 2: Route[0] - Route[End], Layout 3: Route[3] - Route[End]
        {
            new List<(ExternalStationId, List<PlannedFormationId>)>
            {
                (PlannedTrainPartsResolverTests.Route[0].StationId, [PlannedTrainPartsResolverTests.FirstFormationId, PlannedTrainPartsResolverTests.SecondFormationId]),
                (PlannedTrainPartsResolverTests.Route[2].StationId, [PlannedTrainPartsResolverTests.FirstFormationId, PlannedTrainPartsResolverTests.SecondFormationId]),
                (PlannedTrainPartsResolverTests.Route[3].StationId, [PlannedTrainPartsResolverTests.FirstFormationId, PlannedTrainPartsResolverTests.SecondFormationId, PlannedTrainPartsResolverTests.ThirdFormationId]),
                (PlannedTrainPartsResolverTests.Route[^2].StationId, [PlannedTrainPartsResolverTests.FirstFormationId, PlannedTrainPartsResolverTests.SecondFormationId, PlannedTrainPartsResolverTests.ThirdFormationId]),
            },
            new List<PlannedTrainPart>
            {
                new (PlannedTrainPartsResolverTests.Route[0].StationId, PlannedTrainPartsResolverTests.Route[^1].StationId, PlannedTrainPartsResolverTests.FirstFormationId),
                new (PlannedTrainPartsResolverTests.Route[0].StationId, PlannedTrainPartsResolverTests.Route[^1].StationId, PlannedTrainPartsResolverTests.SecondFormationId),
                new (PlannedTrainPartsResolverTests.Route[3].StationId, PlannedTrainPartsResolverTests.Route[^1].StationId, PlannedTrainPartsResolverTests.ThirdFormationId),
            }
        },
        new object[] //Layout 1 + Layout 2: Route[0] - Route[End], Layout 3: Route[3] - Route[End]
        {
            new List<(ExternalStationId, List<PlannedFormationId>)>
            {
                (PlannedTrainPartsResolverTests.Route[0].StationId, [PlannedTrainPartsResolverTests.FirstFormationId, PlannedTrainPartsResolverTests.FirstFormationId]),
                (PlannedTrainPartsResolverTests.Route[2].StationId, [PlannedTrainPartsResolverTests.FirstFormationId, PlannedTrainPartsResolverTests.FirstFormationId]),
                (PlannedTrainPartsResolverTests.Route[3].StationId, [PlannedTrainPartsResolverTests.FirstFormationId, PlannedTrainPartsResolverTests.FirstFormationId, PlannedTrainPartsResolverTests.ThirdFormationId]),
                (PlannedTrainPartsResolverTests.Route[^2].StationId, [PlannedTrainPartsResolverTests.FirstFormationId, PlannedTrainPartsResolverTests.FirstFormationId, PlannedTrainPartsResolverTests.ThirdFormationId]),
            },
            new List<PlannedTrainPart>
            {
                new (PlannedTrainPartsResolverTests.Route[0].StationId, PlannedTrainPartsResolverTests.Route[^1].StationId, PlannedTrainPartsResolverTests.FirstFormationId),
                new (PlannedTrainPartsResolverTests.Route[0].StationId, PlannedTrainPartsResolverTests.Route[^1].StationId, PlannedTrainPartsResolverTests.FirstFormationId),
                new (PlannedTrainPartsResolverTests.Route[3].StationId, PlannedTrainPartsResolverTests.Route[^1].StationId, PlannedTrainPartsResolverTests.ThirdFormationId),
            }
        },
        new object[] //Layout 1: Route[0] - Route[End], Layout 2: Route[3] - Route[End] Layout 3: Route[2] - Route[3]
        {
            new List<(ExternalStationId, List<PlannedFormationId>)>
            {
                (PlannedTrainPartsResolverTests.Route[0].StationId, [PlannedTrainPartsResolverTests.FirstFormationId]),
                (PlannedTrainPartsResolverTests.Route[1].StationId, [PlannedTrainPartsResolverTests.FirstFormationId]),
                (PlannedTrainPartsResolverTests.Route[2].StationId, [PlannedTrainPartsResolverTests.FirstFormationId, PlannedTrainPartsResolverTests.ThirdFormationId]),
                (PlannedTrainPartsResolverTests.Route[3].StationId, [PlannedTrainPartsResolverTests.FirstFormationId, PlannedTrainPartsResolverTests.SecondFormationId, PlannedTrainPartsResolverTests.ThirdFormationId]),
                (PlannedTrainPartsResolverTests.Route[^2].StationId, [PlannedTrainPartsResolverTests.FirstFormationId, PlannedTrainPartsResolverTests.SecondFormationId]),
            },
            new List<PlannedTrainPart>
            {
                new (PlannedTrainPartsResolverTests.Route[0].StationId, PlannedTrainPartsResolverTests.Route[^1].StationId, PlannedTrainPartsResolverTests.FirstFormationId),
                new (PlannedTrainPartsResolverTests.Route[3].StationId, PlannedTrainPartsResolverTests.Route[^1].StationId, PlannedTrainPartsResolverTests.SecondFormationId),
                new (PlannedTrainPartsResolverTests.Route[2].StationId, PlannedTrainPartsResolverTests.Route[4].StationId, PlannedTrainPartsResolverTests.ThirdFormationId),
            }
        }
    };
    public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}