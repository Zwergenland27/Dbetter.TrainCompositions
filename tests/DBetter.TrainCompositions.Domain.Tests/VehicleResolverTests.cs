using DBetter.TrainCompositions.Domain.Vehicles;
using DBetter.TrainCompositions.Domain.Vehicles.ValueObjects;
using NSubstitute;

namespace DBetter.TrainCompositions.Domain.Tests;

public class VehicleResolverTests
{
    private readonly IVehicleRepository _repository;
    
    public VehicleResolverTests()
    {
        _repository = Substitute.For<IVehicleRepository>();
    }

    private static Vehicle MakeVehicle(EuropeanVehicleNumber evn) =>
        Vehicle.Create(evn).Value;

    private static EuropeanVehicleNumber MakeVehicleNumber(string evn) => EuropeanVehicleNumber.Parse(evn).Value;

    [Fact]
    public async Task ResolveMany_ShouldReturnAllExistingWithoutCreating_WhenAllEvnsExist()
    {
        var sut = new VehicleResolver(_repository);
        var evns = new List<EuropeanVehicleNumber>
        {
            MakeVehicleNumber("91 80 6143 958-7"),
            MakeVehicleNumber("50 80 2681 207-5"),
        };
        var existing = evns.Select(MakeVehicle).ToList();
        _repository.GetManyAsync(Arg.Any<IEnumerable<EuropeanVehicleNumber>>()).Returns(existing);

        var result = await sut.ResolveManyAsync(evns);

        Assert.Equal(2, result.Count);
        _repository.DidNotReceive().Store(Arg.Any<Vehicle>());
    }
    
    [Fact]
    public async Task ResolveMany_ShouldCreateAndStoreAll_WhenAllEvnsMissing()
    {
        var sut = new VehicleResolver(_repository);
        var evns = new List<EuropeanVehicleNumber>
        {
            MakeVehicleNumber("91 80 6143 958-7"),
            MakeVehicleNumber("50 80 2681 207-5"),
        };
        _repository.GetManyAsync(Arg.Any<IEnumerable<EuropeanVehicleNumber>>()).Returns([]);

        var result = await sut.ResolveManyAsync(evns);

        Assert.Equal(2, result.Count);
        _repository.Received(2).Store(Arg.Any<Vehicle>());
    }
    
    [Fact]
    public async Task ResolveMany_ShouldCreateOnlyMissing_WhenSomeEvnsMissing()
    {
        var sut = new VehicleResolver(_repository);
        var existingEvn = MakeVehicleNumber("91 80 6143 958-7");
        var missingEvn = MakeVehicleNumber("50 80 2681 207-5");
        var evns = new List<EuropeanVehicleNumber> { existingEvn, missingEvn };
        _repository.GetManyAsync(Arg.Any<IEnumerable<EuropeanVehicleNumber>>()).Returns([MakeVehicle(existingEvn)]);

        var result = await sut.ResolveManyAsync(evns);

        Assert.Equal(2, result.Count);
        _repository.Received(1).Store(Arg.Any<Vehicle>());
    }
    
    [Fact]
    public async Task ResolveMany_ShouldCreateVehicleWithCorrectEvn_WhenSomeEvnsMissing()
    {
        var sut = new VehicleResolver(_repository);
        var existingEvn = MakeVehicleNumber("91 80 6143 958-7");
        var missingEvn = MakeVehicleNumber("50 80 2681 207-5");
        var evns = new List<EuropeanVehicleNumber> { existingEvn, missingEvn };
        _repository.GetManyAsync(Arg.Any<IEnumerable<EuropeanVehicleNumber>>()).Returns([MakeVehicle(existingEvn)]);

        var result = await sut.ResolveManyAsync(evns);

        Assert.Single(result, r => r.Evn == missingEvn);
        var created = result.Single(r => r.Evn == missingEvn);
        Assert.Equal(missingEvn, created.Evn);
    }

   
    [Fact]
    public async Task ResolveMany_ShouldNotCreateAndStoreDuplicateVehicles_WhenDuplicateEvnsNotExist()
    {
        var sut = new VehicleResolver(_repository);
        var missingEvn = MakeVehicleNumber("50 80 2681 207-5");
        var evns = new List<EuropeanVehicleNumber>
        {
            missingEvn,
            MakeVehicleNumber("91 80 6143 958-7")
        };
        _repository.GetManyAsync(Arg.Any<IEnumerable<EuropeanVehicleNumber>>()).Returns([]);
        
        var result = await sut.ResolveManyAsync(evns);

        Assert.Single(result, r => r.Evn == missingEvn);
        _repository.Received(1).Store(Arg.Is<Vehicle>(c => c.Evn == missingEvn));
        var created = result.Single(r => r.Evn == missingEvn);
        Assert.Equal(missingEvn, created.Evn);
    }
    
    [Fact]
    public async Task ResolveMany_ShouldReturnEmpty_WhenInputIsEmpty()
    {
        var sut = new VehicleResolver(_repository);
        _repository.GetManyAsync(Arg.Any<IEnumerable<EuropeanVehicleNumber>>()).Returns([]);

        var result = await sut.ResolveManyAsync([]);

        Assert.Empty(result);
        _repository.DidNotReceive().Store(Arg.Any<Vehicle>());
    }
    
    [Fact]
    public async Task ResolveMany_ShouldReturnBothExistingAndCreated_WhenSomeEvnsMissing()
    {
        var sut = new VehicleResolver(_repository);
        var existingEvn = MakeVehicleNumber("91 80 6143 958-7");
        var missingEvn = MakeVehicleNumber("50 80 2681 207-5");
        var evns = new List<EuropeanVehicleNumber> { existingEvn, missingEvn };
        var existingLayout = MakeVehicle(existingEvn);
        _repository.GetManyAsync(Arg.Any<IEnumerable<EuropeanVehicleNumber>>()).Returns([existingLayout]);

        var result = await sut.ResolveManyAsync(evns);

        Assert.Contains(existingLayout, result);
        Assert.Contains(result, r => r.Evn == missingEvn);
    }

    [Fact]
    public async Task ResolveMany_ShouldNotStore_WhenEvnKnown()
    {
        var knownEvn = MakeVehicleNumber("91 80 6143 958-7");
        var knownVehicle = MakeVehicle(knownEvn);
        var sut = new VehicleResolver(_repository, [knownVehicle]);
        var identifiers = new List<EuropeanVehicleNumber> { knownEvn };
        _repository.GetManyAsync(Arg.Any<IEnumerable<EuropeanVehicleNumber>>()).Returns([]);
        
        _ = await sut.ResolveManyAsync(identifiers);
        
        _repository.DidNotReceive().Store(Arg.Any<Vehicle>());
    }
    
    [Fact]
    public async Task ResolveMany_ShouldNotStoreTwice_WhenDuplicateEvn()
    {
        var evn = MakeVehicleNumber("91 80 6143 958-7");
        var sut = new VehicleResolver(_repository);
        var identifiers = new List<EuropeanVehicleNumber> { evn, evn };
        _repository.GetManyAsync(Arg.Any<IEnumerable<EuropeanVehicleNumber>>()).Returns([]);
        
        _ = await sut.ResolveManyAsync(identifiers);
        
        _repository.Received(1).Store(Arg.Any<Vehicle>());
    }
    
    [Fact]
    public async Task ResolveMany_ShouldAddToKnownList_WhenEvnCreated()
    {
        var knownEvn = MakeVehicleNumber("91 80 6143 958-7");
        var knownVehicles = new List<Vehicle>();
        var sut = new VehicleResolver(_repository, knownVehicles);
        var evns = new List<EuropeanVehicleNumber> { knownEvn };
        _repository.GetManyAsync(Arg.Any<IEnumerable<EuropeanVehicleNumber>>()).Returns([]);
        
        _ = await sut.ResolveManyAsync(evns);
        
        Assert.Single(knownVehicles,  l => l.Evn == knownEvn);
    }
    
    [Fact]
    public async Task ResolveMany_ShouldAddToKnownList_WhenLoadedFromRepository()
    {
        var evn = MakeVehicleNumber("91 80 6143 958-7");
        var knownVehicles = new List<Vehicle>();
        var sut = new VehicleResolver(_repository, knownVehicles);
        var evns = new List<EuropeanVehicleNumber> { evn };
        _repository.GetManyAsync(Arg.Any<IEnumerable<EuropeanVehicleNumber>>()).Returns([MakeVehicle(evn)]);
        
        _ = await sut.ResolveManyAsync(evns);
        
        Assert.Single(knownVehicles,  l => l.Evn == evn);
    }
}