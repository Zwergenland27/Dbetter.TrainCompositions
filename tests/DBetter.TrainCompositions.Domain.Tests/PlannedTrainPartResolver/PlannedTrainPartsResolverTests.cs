using DBetter.TrainCompositions.Domain.PlannedFormations.ValueObjects;
using DBetter.TrainCompositions.Domain.Shared.Stations;
using DBetter.TrainCompositions.Domain.TrainCompositions;
using DBetter.TrainCompositions.Domain.TrainCompositions.TrainParts;

namespace DBetter.TrainCompositions.Domain.Tests.PlannedTrainPartResolver;

public class PlannedTrainPartsResolverTests
{
    public static List<RouteStopSnapshot> Route = [
        new (0, ExternalStationId.Create("8785598c-2f51-44da-ac47-c162b4af91e0").Value),
        new (1, ExternalStationId.Create("20f31a0c-7a39-4375-a6a1-4117ba997cd9").Value),
        new (2, ExternalStationId.Create("497d1216-32f2-438e-a20b-f84d63f1f3e2").Value),
        new (3, ExternalStationId.Create("f85558cb-219a-4e5a-9914-7923bce779a9").Value),
        new (4, ExternalStationId.Create("4b9f8187-0376-4b2c-9f5a-ef694bcf7551").Value),
        new (5, ExternalStationId.Create("47ac9e49-3ce0-44e2-b358-b6e6c3d30b62").Value)
    ];
    
    public static PlannedFormationId FirstFormationId = PlannedFormationId.Create("34fbb2b8-27ef-41cc-a857-ccea22c9dbe9").Value;
    
    public static PlannedFormationId SecondFormationId = PlannedFormationId.Create("6d636966-972f-4475-ac5b-dfb669d2bc5e").Value;
    
    public static PlannedFormationId ThirdFormationId = PlannedFormationId.Create("b47a75ec-d0b4-4140-8405-8c18b1a6c0eb").Value;
    
    [Theory]
    [ClassData(typeof(MissingOriginStopTestData))]
    public void Resolve_ShouldReturnFirstSection_WhenMissing(List<ExternalStationId> observedStations)
    {
        var identifier = new PlannedTrainPartsResolver(Route);
        foreach (var stationId in observedStations)
        {
            identifier.AddObservation(stationId, [FirstFormationId]);
        }
        
        var resolved = identifier.Resolve(out var departureStationToScrape, out var arrivalStationToScrape, out var result);
        
        Assert.False(resolved);
        Assert.Equal(Route[0].StationId, departureStationToScrape);
        Assert.Equal(Route[1].StationId, arrivalStationToScrape);
        Assert.Null(result);
    }
    
    [Theory]
    [ClassData(typeof(MissingDestinationStopTestData))]
    public void Resolve_ShouldReturnLastSection_WhenMissing(List<ExternalStationId> observedStations)
    {
        var identifier = new PlannedTrainPartsResolver(Route);
        identifier.AddObservation(Route[0].StationId, [FirstFormationId]);
        foreach (var stationId in observedStations)
        {
            identifier.AddObservation(stationId, [FirstFormationId]);
        }
        
        var resolved = identifier.Resolve(out var departureStationToScrape, out var arrivalStationToScrape, out var result);
        
        Assert.False(resolved);
        Assert.Equal(Route[^2].StationId, departureStationToScrape);
        Assert.Equal(Route[^1].StationId, arrivalStationToScrape);
        Assert.Null(result);
    }

    [Theory]
    [ClassData(typeof(SimpleTrainPartsTestData))]
    public void Resolve_ShouldReturnResult_WhenFirstAndLastAreEqual(List<PlannedFormationId> formationIds)
    {
        var identifier = new PlannedTrainPartsResolver(Route);
        identifier.AddObservation(Route[0].StationId, formationIds);
        identifier.AddObservation(Route[^2].StationId, formationIds);

        var expectedResult = new List<PlannedTrainPart>();
        foreach (var layoutId in formationIds)
        {
            expectedResult.Add(new (Route[0].StationId, Route[^1].StationId, layoutId));
        }
        
        var resolved = identifier.Resolve(out var departureStationToScrape, out var arrivalStationToScrape, out var result);
        
        Assert.True(resolved);
        Assert.Null(departureStationToScrape);
        Assert.Null(arrivalStationToScrape);
        Assert.Equivalent(expectedResult.OrderBy(r => r.PlannedFormationId.Value),  result!.OrderBy(r => r.PlannedFormationId.Value));
    }

    [Theory]
    [ClassData(typeof(AmbigousMiddleStationTestData))]
    public void Resolve_ShouldReturnMiddleStop_WhenAmbiguousTrainPartsFound(List<(ExternalStationId, List<PlannedFormationId>)> observations, ExternalStationId expectedDepartureStation, ExternalStationId expectedArrivalStation)
    {
        var identifier = new PlannedTrainPartsResolver(Route);
        foreach (var (stationId, coachLayoutIds) in observations)
        {
            identifier.AddObservation(stationId, coachLayoutIds);
        }
        
        var resolved = identifier.Resolve(out var departureStationToScrape, out var arrivalStationToScrape, out var result);
        
        Assert.False(resolved);
        Assert.Equal(expectedDepartureStation, departureStationToScrape);
        Assert.Equal(expectedArrivalStation, arrivalStationToScrape);
        Assert.Null(result);
    }

    [Theory]
    [ClassData(typeof(ResolvedAmbigousMiddleStationTestData))]
    public void Resolve_ShouldReturnResult_WhenTwoFollowingStationsFound(
        List<(ExternalStationId, List<PlannedFormationId>)> observations, List<PlannedTrainPart> expectedResult)
    {
        var identifier = new PlannedTrainPartsResolver(Route);
        foreach (var (stationId, coachLayoutIds) in observations)
        {
            identifier.AddObservation(stationId, coachLayoutIds);
        }
        
        var resolved = identifier.Resolve(out var departureStationToScrape, out var arrivalStationToScrape, out var result);
        
        Assert.True(resolved);
        Assert.Null(departureStationToScrape);
        Assert.Null(arrivalStationToScrape);
        Assert.Equivalent(expectedResult.OrderBy(r => r.PlannedFormationId.Value),  result!.OrderBy(r => r.PlannedFormationId.Value));
    }
}