using DBetter.TrainCompositions.Domain.CoachLayouts;
using DBetter.TrainCompositions.Domain.CoachLayouts.ValueObjects;
using NSubstitute;

namespace DBetter.TrainCompositions.Domain.Tests;

public class CoachLayoutResolverTests
{
    private readonly ICoachLayoutRepository _repository;
    
    public CoachLayoutResolverTests()
    {
        _repository = Substitute.For<ICoachLayoutRepository>();
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
        var sut = new CoachLayoutResolver(_repository);
        var identifiers = new List<CoachLayoutIdentifier>
        {
            new("I8044"),
            new("I8042"),
        };
        var existing = identifiers.Select(i => MakeCoachLayout(i.Value)).ToList();
        _repository.FindManyAsync(Arg.Any<IEnumerable<CoachLayoutIdentifier>>()).Returns(existing);

        var result = await sut.ResolveManyAsync(identifiers);

        Assert.Equal(2, result.Count);
        _repository.DidNotReceive().Store(Arg.Any<CoachLayout>());
    }
    
    [Fact]
    public async Task ResolveMany_ShouldCreateAndStoreAll_WhenAllIdentifiersMissing()
    {
        var sut = new CoachLayoutResolver(_repository);
        var identifiers = new List<CoachLayoutIdentifier>
        {
            new("I8044"),
            new("I8042"),
        };
        _repository.FindManyAsync(Arg.Any<IEnumerable<CoachLayoutIdentifier>>()).Returns([]);

        var result = await sut.ResolveManyAsync(identifiers);

        Assert.Equal(2, result.Count);
        _repository.Received(2).Store(Arg.Any<CoachLayout>());
    }
    
    [Fact]
    public async Task ResolveMany_ShouldCreateOnlyMissing_WhenSomeIdentifiersMissing()
    {
        var sut = new CoachLayoutResolver(_repository);
        var existingIdentifier = new CoachLayoutIdentifier("I8042");
        var missingIdentifier = new CoachLayoutIdentifier("I8044");
        var identifiers = new List<CoachLayoutIdentifier> { existingIdentifier, missingIdentifier };
        _repository.FindManyAsync(Arg.Any<IEnumerable<CoachLayoutIdentifier>>()).Returns([MakeCoachLayout("I8042")]);

        var result = await sut.ResolveManyAsync(identifiers);

        Assert.Equal(2, result.Count);
        _repository.Received(1).Store(Arg.Any<CoachLayout>());
    }
    
    [Fact]
    public async Task ResolveMany_ShouldCreateLayoutWithCorrectIdentifier_WhenSomeIdentifiersMissing()
    {
        var sut = new CoachLayoutResolver(_repository);
        var missingIdentifier = new CoachLayoutIdentifier("I8044");
        var identifiers = new List<CoachLayoutIdentifier>
        {
            new("I8042"),
            missingIdentifier,
        };
        _repository.FindManyAsync(Arg.Any<IEnumerable<CoachLayoutIdentifier>>()).Returns([MakeCoachLayout("I8042")]);

        var result = await sut.ResolveManyAsync(identifiers);

        Assert.Single(result, r => r.Identifier == missingIdentifier);
        var created = result.Single(r => r.Identifier == missingIdentifier);
        Assert.Equal(missingIdentifier, created.Identifier);
    }

   
    [Fact]
    public async Task ResolveMany_ShouldNotCreateAndStoreDuplicateLayouts_WhenDuplicateIdentifiersNotExist()
    {
        var sut = new CoachLayoutResolver(_repository);
        var missingIdentifier = new CoachLayoutIdentifier("I8044");
        var identifiers = new List<CoachLayoutIdentifier>
        {
            missingIdentifier,
            new ("I8044")
        };
        _repository.FindManyAsync(Arg.Any<IEnumerable<CoachLayoutIdentifier>>()).Returns([]);
        
        var result = await sut.ResolveManyAsync(identifiers);

        Assert.Single(result, r => r.Identifier == missingIdentifier);
        _repository.Received(1).Store(Arg.Is<CoachLayout>(c => c.Identifier == missingIdentifier));
        var created = result.Single(r => r.Identifier == missingIdentifier);
        Assert.Equal(missingIdentifier, created.Identifier);
    }
    
    [Fact]
    public async Task ResolveMany_ShouldReturnEmpty_WhenInputIsEmpty()
    {
        var sut = new CoachLayoutResolver(_repository);
        _repository.FindManyAsync(Arg.Any<IEnumerable<CoachLayoutIdentifier>>()).Returns([]);

        var result = await sut.ResolveManyAsync([]);

        Assert.Empty(result);
        _repository.DidNotReceive().Store(Arg.Any<CoachLayout>());
    }
    
    [Fact]
    public async Task ResolveMany_ShouldReturnBothExistingAndCreated_WhenSomeIdentifiersMissing()
    {
        var sut = new CoachLayoutResolver(_repository);
        var existingIdentifier = new CoachLayoutIdentifier("I8082");
        var missingIdentifier = new CoachLayoutIdentifier("I8044");
        var identifiers = new List<CoachLayoutIdentifier> { existingIdentifier, missingIdentifier };
        var existingLayout = MakeCoachLayout("I8082");
        _repository.FindManyAsync(Arg.Any<IEnumerable<CoachLayoutIdentifier>>()).Returns([existingLayout]);

        var result = await sut.ResolveManyAsync(identifiers);

        Assert.Contains(existingLayout, result);
        Assert.Contains(result, r => r.Identifier == missingIdentifier);
    }

    [Fact]
    public async Task ResolveMany_ShouldNotStore_WhenIdentifierKnown()
    {
        var knownIdentifier = new CoachLayoutIdentifier("I8082");
        var knownCoachLayout = CoachLayout.CreateFromPlanned(knownIdentifier);
        var sut = new CoachLayoutResolver(_repository, [knownCoachLayout]);
        var identifiers = new List<CoachLayoutIdentifier> { knownIdentifier };
        _repository.FindManyAsync(Arg.Any<IEnumerable<CoachLayoutIdentifier>>()).Returns([]);
        
        _ = await sut.ResolveManyAsync(identifiers);
        
        _repository.DidNotReceive().Store(Arg.Any<CoachLayout>());
    }
    
    [Fact]
    public async Task ResolveMany_ShouldNotStoreTwice_WhenDuplicateIdentifier()
    {
        var identifier = new CoachLayoutIdentifier("I8082");
        var sut = new CoachLayoutResolver(_repository);
        var identifiers = new List<CoachLayoutIdentifier> { identifier, identifier };
        _repository.FindManyAsync(Arg.Any<IEnumerable<CoachLayoutIdentifier>>()).Returns([]);
        
        _ = await sut.ResolveManyAsync(identifiers);
        
        _repository.Received(1).Store(Arg.Any<CoachLayout>());
    }
    
    [Fact]
    public async Task ResolveMany_ShouldAddToKnownList_WhenIdentifierCreated()
    {
        var identifier = new CoachLayoutIdentifier("I8082");
        var knownCoachLayouts = new List<CoachLayout>();
        var sut = new CoachLayoutResolver(_repository, knownCoachLayouts);
        var identifiers = new List<CoachLayoutIdentifier> { identifier };
        _repository.FindManyAsync(Arg.Any<IEnumerable<CoachLayoutIdentifier>>()).Returns([]);
        
        _ = await sut.ResolveManyAsync(identifiers);
        
        Assert.Single(knownCoachLayouts,  l => l.Identifier == identifier);
    }
    
    [Fact]
    public async Task ResolveMany_ShouldAddToKnownList_WhenLoadedFromRepository()
    {
        var identifier = new CoachLayoutIdentifier("I8082");
        var knownCoachLayouts = new List<CoachLayout>();
        var sut = new CoachLayoutResolver(_repository, knownCoachLayouts);
        var identifiers = new List<CoachLayoutIdentifier> { identifier };
        _repository.FindManyAsync(Arg.Any<IEnumerable<CoachLayoutIdentifier>>()).Returns([MakeCoachLayout("I8082")]);
        
        _ = await sut.ResolveManyAsync(identifiers);
        
        Assert.Single(knownCoachLayouts,  l => l.Identifier == identifier);
    }
}