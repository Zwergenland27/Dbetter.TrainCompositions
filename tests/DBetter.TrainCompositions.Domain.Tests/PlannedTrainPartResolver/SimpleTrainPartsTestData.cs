using System.Collections;
using DBetter.TrainCompositions.Domain.PlannedFormations.ValueObjects;

namespace DBetter.TrainCompositions.Domain.Tests.PlannedTrainPartResolver;

public class SimpleTrainPartsTestData: IEnumerable<object[]>
{
    private readonly List<object[]> _data = new List<object[]>
    {
        new object[]{new List<PlannedFormationId>(){PlannedTrainPartsResolverTests.FirstFormationId}},
        new object[]{new List<PlannedFormationId>(){PlannedTrainPartsResolverTests.SecondFormationId}},
        new object[]{new List<PlannedFormationId>(){PlannedTrainPartsResolverTests.FirstFormationId, PlannedTrainPartsResolverTests.SecondFormationId}},
        new object[]{new List<PlannedFormationId>(){PlannedTrainPartsResolverTests.FirstFormationId, PlannedTrainPartsResolverTests.FirstFormationId}},
    };
    public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}