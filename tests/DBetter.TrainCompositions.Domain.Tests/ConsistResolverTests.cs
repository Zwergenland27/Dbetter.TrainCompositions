using DBetter.TrainCompositions.Domain.Consists;
using DBetter.TrainCompositions.Domain.Vehicles.ValueObjects;
using NSubstitute;

namespace DBetter.TrainCompositions.Domain.Tests;

public class ConsistResolverTests
{
    private readonly IConsistRepository _repository;

    public ConsistResolverTests()
    {
        _repository = Substitute.For<IConsistRepository>();
    }

    private static Consist MakeConsist(List<VehicleId> vehicleIds) => Consist.Create(vehicleIds).Value;
    
    [Fact]
    public async Task ResolveAsync_ShouldReturnKnownConsist_WhenMatchExistsInKnownConsists()
    {
        var vehicleIds = new List<VehicleId> { VehicleId.CreateNew() };
        var knownConsist = MakeConsist(vehicleIds);
        var resolver = new ConsistResolver(_repository, [knownConsist]);

        var result = await resolver.ResolveAsync(vehicleIds);

        Assert.Equal(knownConsist, result);
        await _repository.DidNotReceive().FindAsync(Arg.Any<List<VehicleId>>());
    }
    
    [Fact]
    public async Task ResolveAsync_ShouldReturnConsist_WhenFoundInRepository()
    {
        var vehicleIds = new List<VehicleId> { VehicleId.CreateNew() };
        var existingConsist = MakeConsist(vehicleIds);
        _repository.FindAsync(vehicleIds).Returns(existingConsist);
        var resolver = new ConsistResolver(_repository);
        
        var result = await resolver.ResolveAsync(vehicleIds);
        
        Assert.Equal(existingConsist, result);
    }
    
    [Fact]
    public async Task ResolveAsync_ShouldAddToKnown_WhenFoundInRepository()
    {
        var vehicleIds = new List<VehicleId> { VehicleId.CreateNew() };
        var existingConsist = MakeConsist(vehicleIds);
        _repository.FindAsync(vehicleIds).Returns(existingConsist);
        var resolver = new ConsistResolver(_repository);
        
        _ = await resolver.ResolveAsync(vehicleIds);
        
        Assert.Contains(resolver.AllKnownConsists, x => x == existingConsist);
        _repository.DidNotReceive().Store(Arg.Any<Consist>());
    }
    
    [Fact]
    public async Task ResolveAsync_ShouldNotStore_WhenFoundInRepository()
    {
        var vehicleIds = new List<VehicleId> { VehicleId.CreateNew() };
        var existingConsist = MakeConsist(vehicleIds);
        _repository.FindAsync(vehicleIds).Returns(existingConsist);
        var resolver = new ConsistResolver(_repository);
        
        _ = await resolver.ResolveAsync(vehicleIds);
        
        _repository.DidNotReceive().Store(Arg.Any<Consist>());
    }

    [Fact]
    public async Task ResolveAsync_ShouldReturnCreatedConsist_WhenNotExisting()
    {
        var vehicleIds = new List<VehicleId> { VehicleId.CreateNew() };
        _repository.FindAsync(vehicleIds).Returns((Consist?) null);
        var resolver = new ConsistResolver(_repository);
        
        var result = await resolver.ResolveAsync(vehicleIds);
        
        Assert.NotNull(result);
        Assert.Equivalent(vehicleIds, result.Vehicles.Select(x => x.VehicleId));
    }
    
    [Fact]
    public async Task ResolveAsync_ShouldAddToKnown_WhenNotExisting()
    {
        var vehicleIds = new List<VehicleId> { VehicleId.CreateNew() };
        _repository.FindAsync(vehicleIds).Returns((Consist?) null);
        var resolver = new ConsistResolver(_repository);
        
        var result = await resolver.ResolveAsync(vehicleIds);
        
        Assert.NotNull(result);
        Assert.Contains(resolver.AllKnownConsists, x => x.Id == result.Id);
    }
    
    [Fact]
    public async Task ResolveAsync_ShouldCallStore_WhenNotExisting()
    {
        var vehicleIds = new List<VehicleId> { VehicleId.CreateNew() };
        _repository.FindAsync(vehicleIds).Returns((Consist?) null);
        var resolver = new ConsistResolver(_repository);
        
        _ = await resolver.ResolveAsync(vehicleIds);
        
        _repository.Received().Store(Arg.Any<Consist>());
    }
}