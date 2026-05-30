using DBetter.TrainCompositions.Application.Ports.Formations;
using DBetter.TrainCompositions.Application.Ports.TrainRuns;
using DBetter.TrainCompositions.Application.TrainCompositions.Fetch;
using DBetter.TrainCompositions.Domain.CoachLayouts;
using DBetter.TrainCompositions.Domain.CoachLayouts.ValueObjects;
using DBetter.TrainCompositions.Domain.PlannedFormations;
using DBetter.TrainCompositions.Domain.PlannedFormations.ValueObjects;
using DBetter.TrainCompositions.Domain.Shared.Stations;
using DBetter.TrainCompositions.Domain.TrainCompositions;
using DBetter.TrainCompositions.Domain.TrainCompositions.TrainParts;
using NSubstitute;

namespace DBetter.TrainCompositions.Application.Tests;

public class PlannedRouteFormationResolverTests
{
    private static ExternalStationId StationId(Guid? id = null) =>
        new(id ?? Guid.NewGuid());

    private static TrainRunStopDto MakeStop(
        ExternalStationId id,
        int routeIndex,
        string eva = "8000001",
        DateTime? departure = null,
        DateTime? arrival = null) =>
        new()
        {
            Id                   = id,
            RouteIndex           = routeIndex,
            EvaNumber            = eva,
            PlannedDepartureTime = departure ?? DateTime.UtcNow.AddHours(routeIndex),
            PlannedArrivalTime   = arrival  ?? DateTime.UtcNow.AddHours(routeIndex + 1),
        };
 
    private static TrainRunRouteDto MakeRoute(
        int serviceNumber,
        IEnumerable<TrainRunStopDto> stops) =>
        new()
        {
            ServiceNumber = serviceNumber,
            Stops         = stops.ToList(),
        };
 
    private static CoachLayoutIdentifier CoachIdentifier(string value = "coach-1") =>
        CoachLayoutIdentifier.Create(value);

    private static CoachLayout MakeCoachLayout(CoachLayoutIdentifier identifier) =>
        CoachLayout.CreateFromPlanned(identifier);

    private static PlannedFormation MakePlannedFormation(
        PlannedFormationId id,
        List<CoachLayoutId> coachLayoutIds) =>
        PlannedFormation.Create(new PlannedFormationSnapshot(coachLayoutIds)).Value;

    private static IPlannedTrainPartsResolver MakePlannedTrainPartsResolver(TrainRunRouteDto route)
    {
        var routeStops = route.Stops
            .Select(s => new RouteStopSnapshot(s.RouteIndex, s.Id))
            .ToList();
        return new PlannedTrainPartsResolver(routeStops);
    }
    
    private static PlannedVehicleDto MakeVehicleDto(params CoachLayoutIdentifier[] coaches) =>
        new() { CoachIdentifiers = coaches.ToList() };

    
 
    private static PlannedRouteFormationResolver CreateSut(
        ICoachLayoutResolver?      coachLayoutResolver      = null,
        IPlannedFormationResolver? plannedFormationResolver = null,
        IPlannedFormationProvider? plannedFormationProvider = null) =>
        new(
            coachLayoutResolver      ?? Substitute.For<ICoachLayoutResolver>(),
            plannedFormationResolver ?? Substitute.For<IPlannedFormationResolver>(),
            plannedFormationProvider ?? Substitute.For<IPlannedFormationProvider>());
    
    [Fact]
    public async Task GetPlannedAsync_ShouldReturnPlannedNotAvailableError_WhenServiceNumberIsNull()
    {
        var sut   = CreateSut();
        var route = new TrainRunRouteDto { ServiceNumber = null, Stops = [] };
        var resolver = Substitute.For<IPlannedTrainPartsResolver>();
 
        var result = await sut.GetPlannedAsync(route, resolver);
 
        Assert.True(result.HasFailed);
        Assert.Single(result.Errors, TrainCompositionErrors.PlannedNotAvailable);
    }
    
    [Fact]
    public async Task GetPlannedAsync_ShouldReturnInsufficientDataError_WhenProviderReturnsNullOnFirstCall()
    {
        var stationA = StationId();
        var stationB = StationId();
 
        var stopA = MakeStop(stationA, routeIndex: 0, eva: "8000001");
        var stopB = MakeStop(stationB, routeIndex: 1, eva: "8000002");
 
        var provider = Substitute.For<IPlannedFormationProvider>();
        provider
            .GetForSectionAsync(1234, stopA.EvaNumber, stopA.PlannedDepartureTime!.Value,
                stopB.EvaNumber, stopB.PlannedArrivalTime!.Value)
            .Returns((List<PlannedVehicleDto>?)null);
 
        var sut  = CreateSut(plannedFormationProvider: provider);
        var route = MakeRoute(serviceNumber: 1234, stops: [stopA, stopB]);

        var trainPartsResolver = MakePlannedTrainPartsResolver(route);
        
        var result = await sut.GetPlannedAsync(route, trainPartsResolver);
        
        Assert.True(result.HasFailed);
        Assert.Single(result.Errors, TrainCompositionErrors.InsufficientData);
    }
    
    [Fact]
    public async Task GetPlannedAsync_ShouldReturnInsufficientDataError_WhenProviderReturnsNullOnSecondCall()
    {
        var stationA = StationId();
        var stationB = StationId();
        var stationC = StationId();
 
        var stopA = MakeStop(stationA, routeIndex: 0, eva: "8000001");
        var stopB = MakeStop(stationB, routeIndex: 1, eva: "8000002");
        var stopC = MakeStop(stationC, routeIndex: 2, eva: "8000003");

        var coachIdentifier = CoachIdentifier();
        var layout    = MakeCoachLayout(coachIdentifier);
        var formation = MakePlannedFormation(PlannedFormationId.CreateNew(), [layout.Id]);
        
        var provider = Substitute.For<IPlannedFormationProvider>();
        provider
            .GetForSectionAsync(1234, stopA.EvaNumber, stopA.PlannedDepartureTime!.Value,
                stopB.EvaNumber, stopB.PlannedArrivalTime!.Value)
            .Returns([MakeVehicleDto(coachIdentifier)]);
        provider
            .GetForSectionAsync(1234, stopB.EvaNumber, stopB.PlannedDepartureTime!.Value,
                stopC.EvaNumber, stopC.PlannedArrivalTime!.Value)
            .Returns((List<PlannedVehicleDto>?)null);
        
        var coachLayoutResolver = Substitute.For<ICoachLayoutResolver>();
        coachLayoutResolver
            .ResolveManyAsync(Arg.Any<List<CoachLayoutIdentifier>>())
            .Returns([layout]);
 
        var formationResolver = Substitute.For<IPlannedFormationResolver>();
        formationResolver
            .ResolveManyAsync(Arg.Any<List<PlannedFormationSnapshot>>())
            .Returns([formation]);
 
        var sut  = CreateSut(coachLayoutResolver, formationResolver, provider);
        var route = MakeRoute(serviceNumber: 1234, stops: [stopA, stopB, stopC]);
        
        var trainPartsResolver = MakePlannedTrainPartsResolver(route);
 
        var result = await sut.GetPlannedAsync(route, trainPartsResolver);
        
        Assert.True(result.HasFailed);
        Assert.Single(result.Errors, TrainCompositionErrors.InsufficientData);
    }

    [Fact]
    public async Task GetPlannedAsync_ShouldAddObservation_WhenProviderReturnsPlannedTrainComposition()
    {
        var stationA = StationId();
        var stationB = StationId();
 
        var stopA = MakeStop(stationA, routeIndex: 0, eva: "8000001");
        var stopB = MakeStop(stationB, routeIndex: 1, eva: "8000002");
 
        var provider = Substitute.For<IPlannedFormationProvider>();
        provider
            .GetForSectionAsync(1234, stopA.EvaNumber, stopA.PlannedDepartureTime!.Value,
                stopB.EvaNumber, stopB.PlannedArrivalTime!.Value)
            .Returns((List<PlannedVehicleDto>?)null);
 
        var sut  = CreateSut(plannedFormationProvider: provider);
        var route = MakeRoute(serviceNumber: 1234, stops: [stopA, stopB]);
        
        var trainPartsResolver = MakePlannedTrainPartsResolver(route);
 
        var result = await sut.GetPlannedAsync(route, trainPartsResolver);
        
        Assert.True(result.HasFailed);
        Assert.Single(result.Errors, TrainCompositionErrors.InsufficientData);
    }
    
    [Fact]
    public async Task GetPlannedAsync_ShouldReturnResult_WhenSingleSectionResolvedSuccessfully()
    {
        var stationA = StationId();
        var stationB = StationId();
        var stationC = StationId();
    
        var stopA = MakeStop(stationA, routeIndex: 0, eva: "8000001");
        var stopB = MakeStop(stationB, routeIndex: 1, eva: "8000002");
        var stopC = MakeStop(stationC, routeIndex: 2, eva: "8000003");
    
        var coachIdentifier = CoachIdentifier();
        var layout = MakeCoachLayout(coachIdentifier);
        var formation = MakePlannedFormation(new PlannedFormationId(Guid.NewGuid()), [layout.Id]);
        var trainParts = new List<PlannedTrainPart> { new(stationA, stationC, formation.Id) };
    
        var provider = Substitute.For<IPlannedFormationProvider>();
        provider
            .GetForSectionAsync(1234, stopA.EvaNumber, stopA.PlannedDepartureTime!.Value,
                stopB.EvaNumber, stopB.PlannedArrivalTime!.Value)
            .Returns([MakeVehicleDto(coachIdentifier)]);
        
        provider
            .GetForSectionAsync(1234, stopB.EvaNumber, stopB.PlannedDepartureTime!.Value,
                stopC.EvaNumber, stopC.PlannedArrivalTime!.Value)
            .Returns([MakeVehicleDto(coachIdentifier)]);
    
        var coachLayoutResolver = Substitute.For<ICoachLayoutResolver>();
        coachLayoutResolver
            .ResolveManyAsync(Arg.Is<List<CoachLayoutIdentifier>>(l => l.Contains(coachIdentifier)))
            .Returns([layout]);
    
        var formationResolver = Substitute.For<IPlannedFormationResolver>();
        formationResolver
            .ResolveManyAsync(Arg.Any<List<PlannedFormationSnapshot>>())
            .Returns([formation]);
    
        var sut    = CreateSut(coachLayoutResolver, formationResolver, provider);
        var route  = MakeRoute(serviceNumber: 1234, stops: [stopA, stopB, stopC]);
        var trainPartsResolver = MakePlannedTrainPartsResolver(route);
        
        var result = await sut.GetPlannedAsync(route, trainPartsResolver);
    
        Assert.False(result.HasFailed);
        Assert.Equivalent(trainParts, result.Value.PlannedTrainParts);
    }
}