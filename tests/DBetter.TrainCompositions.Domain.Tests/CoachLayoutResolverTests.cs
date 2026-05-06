using DBetter.TrainCompositions.Domain.CoachLayouts;
using DBetter.TrainCompositions.Domain.CoachLayouts.ValueObjects;
using NSubstitute;

namespace DBetter.TrainCompositions.Domain.Tests;

public class CoachLayoutResolverTests
{
    private readonly ICoachLayoutRepository _repository;
    private readonly CoachLayoutResolver _sut;
    
    public CoachLayoutResolverTests()
    {
        _repository = Substitute.For<ICoachLayoutRepository>();
        _sut = new CoachLayoutResolver(_repository);
    }
    
    private static CoachLayout MakeCoachLayout(string identifier) =>
        new CoachLayout(
            new CoachLayoutId(Guid.NewGuid()),
            new CoachLayoutIdentifier(identifier),
            null,
            new Amenities());

    [Fact]
    public async Task ResolveMany_ShouldReturnAllExistingWithoutCreating_WhenAllIdentifiersExist()
    {
        var identifiers = new List<CoachLayoutIdentifier>
        {
            new("I8044"),
            new("I8042"),
        };
        var existing = identifiers.Select(i => MakeCoachLayout(i.Value)).ToList();
        _repository.FindManyAsync(identifiers).Returns(existing);

        var result = await _sut.ResolveMany(identifiers);

        Assert.Equal(2, result.Count);
        _repository.DidNotReceive().Store(Arg.Any<CoachLayout>());
    }
    
    [Fact]
    public async Task ResolveMany_ShouldCreateAndStoreAll_WhenAllIdentifiersMissing()
    {
        var identifiers = new List<CoachLayoutIdentifier>
        {
            new("I8044"),
            new("I8042"),
        };
        _repository.FindManyAsync(identifiers).Returns([]);

        var result = await _sut.ResolveMany(identifiers);

        Assert.Equal(2, result.Count);
        _repository.Received(2).Store(Arg.Any<CoachLayout>());
    }
    
    [Fact]
    public async Task ResolveMany_ShouldCreateOnlyMissing_WhenSomeIdentifiersMissing()
    {
        var existingIdentifier = new CoachLayoutIdentifier("I8042");
        var missingIdentifier = new CoachLayoutIdentifier("I8044");
        var identifiers = new List<CoachLayoutIdentifier> { existingIdentifier, missingIdentifier };
        _repository.FindManyAsync(identifiers).Returns([MakeCoachLayout("I8042")]);

        var result = await _sut.ResolveMany(identifiers);

        Assert.Equal(2, result.Count);
        _repository.Received(1).Store(Arg.Any<CoachLayout>());
    }
    
    [Fact]
    public async Task ResolveMany_ShouldCreateLayoutWithCorrectIdentifier_WhenSomeIdentifierIsMissing()
    {
        var missingIdentifier = new CoachLayoutIdentifier("I8044");
        var identifiers = new List<CoachLayoutIdentifier>
        {
            new("I8042"),
            missingIdentifier,
        };
        _repository.FindManyAsync(identifiers).Returns([MakeCoachLayout("I8042")]);

        var result = await _sut.ResolveMany(identifiers);

        var created = result.Single(r => r.Identifier == missingIdentifier);
        Assert.Equal(missingIdentifier, created.Identifier);
    }
    
    [Fact]
    public async Task ResolveMany_ShouldReturnEmpty_WhenInputIsEmpty()
    {
        _repository.FindManyAsync(Arg.Any<List<CoachLayoutIdentifier>>()).Returns([]);

        var result = await _sut.ResolveMany([]);

        Assert.Empty(result);
        _repository.DidNotReceive().Store(Arg.Any<CoachLayout>());
    }
    
    [Fact]
    public async Task ResolveMany_ShouldReturnBothExistingAndCreated_WhenSomeIdentifiersMissing()
    {
        var existingIdentifier = new CoachLayoutIdentifier("I8082");
        var missingIdentifier = new CoachLayoutIdentifier("I8044");
        var identifiers = new List<CoachLayoutIdentifier> { existingIdentifier, missingIdentifier };
        var existingLayout = MakeCoachLayout("I8082");
        _repository.FindManyAsync(identifiers).Returns([existingLayout]);

        var result = await _sut.ResolveMany(identifiers);

        Assert.Contains(existingLayout, result);
        Assert.Contains(result, r => r.Identifier == missingIdentifier);
    }
    
    [Fact]
    public async Task ResolveMany_ShouldReturnLayoutsInSameOrder_WhenAllIdentifiersExist()
    {
        var identifiers = new List<CoachLayoutIdentifier>
        {
            new("I8022"),
            new("I8024"),
            new("I8044"),
        };
        _repository.FindManyAsync(Arg.Any<List<CoachLayoutIdentifier>>())
            .Returns(identifiers.AsEnumerable().Reverse().Select(i => MakeCoachLayout(i.Value)).ToList());

        var result = await _sut.ResolveMany(identifiers);

        Assert.Equal(identifiers.Select(i => i.Value), result.Select(r => r.Identifier.Value));
    }

    [Fact]
    public async Task ResolveMany_ShouldReturnLayoutsInSameOrder_WhenAllIdentifiersMissing()
    {
        var identifiers = new List<CoachLayoutIdentifier>
        {
            new("I8022"),
            new("I8024"),
            new("I8044"),
        };
        _repository.FindManyAsync(Arg.Any<List<CoachLayoutIdentifier>>()).Returns([]);

        var result = await _sut.ResolveMany(identifiers);

        Assert.Equal(identifiers.Select(i => i.Value), result.Select(r => r.Identifier.Value));
    }

    [Fact]
    public async Task ResolveMany_ShouldReturnLayoutsInSameOrder_WhenSomeIdentifiersMissing()
    {
        var identifiers = new List<CoachLayoutIdentifier>
        {
            new("I8022"),
            new("I8024"),
            new("I8044"),
        };
        
        _repository.FindManyAsync(Arg.Any<List<CoachLayoutIdentifier>>())
            .Returns([MakeCoachLayout("I8024")]);

        var result = await _sut.ResolveMany(identifiers);

        Assert.Equal(identifiers.Select(i => i.Value), result.Select(r => r.Identifier.Value));
    }
}