using DBetter.TrainCompositions.Domain.CoachLayouts.ValueObjects;
using DBetter.TrainCompositions.Domain.PlannedFormations;
using DBetter.TrainCompositions.Domain.PlannedFormations.Coaches;
using DBetter.TrainCompositions.Domain.PlannedFormations.Coaches.ValueObjects;
using DBetter.TrainCompositions.Domain.PlannedFormations.ValueObjects;
using NSubstitute;

namespace DBetter.TrainCompositions.Domain.Tests;

public class PlannedFormationResolverTests
{
    private readonly IPlannedFormationRepository _repository;

    public PlannedFormationResolverTests()
    {
        _repository = Substitute.For<IPlannedFormationRepository>();
    }
    private static PlannedFormationSnapshot Snapshot(params CoachLayoutId[] coaches)
        => new(coaches.ToList());

    private static PlannedFormation ExistingFormation(params CoachLayoutId[] coaches)
    {
        var positions = coaches
            .Select((id, i) => new PlannedCoach(new PlannedCoachPosition((short)(i + 1)), id))
            .ToList();
        return new PlannedFormation(PlannedFormationId.CreateNew(), positions);
    }

    [Fact]
    public async Task ResolveManyAsync_ShouldReturnEmpty_WhenInputIsEmpty()
    {
        var sut = new PlannedFormationResolver(_repository, []);
        _repository.FindManyAsync(Arg.Any<List<PlannedFormationSnapshot>>()).Returns([]);

        var result = await sut.ResolveManyAsync([]);
        
        Assert.Empty(result);
    }

    [Fact]
    public async Task ResolveManyAsync_ShouldReturnExisting_WhenAllSequencesAlreadyExist()
    {
        var sut = new PlannedFormationResolver(_repository, []);
        var coachId = CoachLayoutId.CreateNew();
        var snapshot = Snapshot(coachId);
        var existing = ExistingFormation(coachId);
        var snapshots = new List<PlannedFormationSnapshot>{snapshot};

        _repository.FindManyAsync(Arg.Any<IEnumerable<PlannedFormationSnapshot>>()).Returns([existing]);

        var result = await sut.ResolveManyAsync(snapshots);
        
        Assert.Single(result, existing);
        _repository.DidNotReceive().Store(Arg.Any<PlannedFormation>());
    }

    [Fact]
    public async Task ResolveManyAsync_ShouldCreateAndStore_WhenSequenceDoesNotExist()
    {
        var sut = new PlannedFormationResolver(_repository, []);
        var snapshot = Snapshot(CoachLayoutId.CreateNew());
        var snapshots = new List<PlannedFormationSnapshot>{snapshot};

        _repository.FindManyAsync(Arg.Any<IEnumerable<PlannedFormationSnapshot>>()).Returns([]);

        var result = await sut.ResolveManyAsync(snapshots);
        
        Assert.Single(result);
        _repository.Received(1).Store(Arg.Any<PlannedFormation>());
    }
    
    [Fact]
    public async Task ResolveManyAsync_ShouldNotCreateAndStoreDuplicate_WhenDuplicateSequenceDoesNotExist()
    {
        var sut = new PlannedFormationResolver(_repository, []);
        var coachLayoutId =  CoachLayoutId.CreateNew();
        var snapshot = Snapshot(coachLayoutId);
        var snapshots = new List<PlannedFormationSnapshot>{snapshot, Snapshot(coachLayoutId)};

        _repository.FindManyAsync(Arg.Any<IEnumerable<PlannedFormationSnapshot>>()).Returns([]);

        var result = await sut.ResolveManyAsync(snapshots);
        
        Assert.Single(result);
        _repository.Received(1).Store(Arg.Is<PlannedFormation>(p => p.Matches(snapshot)));
    }

    [Fact]
    public async Task ResolveManyAsync_ShouldCreateOnlyMissing_WhenSomeSequencesAlreadyExist()
    {
        var sut = new PlannedFormationResolver(_repository, []);
        var existingCoachId = CoachLayoutId.CreateNew();
        var newCoachId = CoachLayoutId.CreateNew();

        var existingSnapshot = Snapshot(existingCoachId);
        var newSnapshot = Snapshot(newCoachId);
        var existing = ExistingFormation(existingCoachId);
        var snapshots = new List<PlannedFormationSnapshot>{existingSnapshot, newSnapshot};

        _repository.FindManyAsync(Arg.Any<IEnumerable<PlannedFormationSnapshot>>()).Returns([existing]);

        var result = await sut.ResolveManyAsync(snapshots);
        
        Assert.Equal(2, result.Count);
        _repository.Received(1).Store(Arg.Any<PlannedFormation>());
    }

    [Fact]
    public async Task ResolveManyAsync_ShouldReturnAllCreated_WhenMultipleNewSequencesGiven()
    {
        var sut = new PlannedFormationResolver(_repository, []);
        var snapshots = Enumerable.Range(0, 3)
            .Select(_ => Snapshot(CoachLayoutId.CreateNew()))
            .ToList();

        _repository.FindManyAsync(Arg.Any<IEnumerable<PlannedFormationSnapshot>>()).Returns([]);

        var result = await sut.ResolveManyAsync(snapshots);
        
        Assert.Equal(3, result.Count);
        _repository.Received(3).Store(Arg.Any<PlannedFormation>());
    }
    
     [Fact]
    public async Task ResolveMany_ShouldNotStore_WhenSequenceKnown()
    {
        var existingCoachLayoutId = CoachLayoutId.CreateNew();
        var knownFormation = ExistingFormation(existingCoachLayoutId); 
        var sut = new PlannedFormationResolver(_repository, [knownFormation]);
        var identifiers = new List<PlannedFormationSnapshot> { Snapshot(existingCoachLayoutId) };
        _repository.FindManyAsync(Arg.Any<IEnumerable<PlannedFormationSnapshot>>()).Returns([]);
        
        _ = await sut.ResolveManyAsync(identifiers);
        
        _repository.DidNotReceive().Store(Arg.Any<PlannedFormation>());
    }
    
    [Fact]
    public async Task ResolveMany_ShouldNotStoreTwice_WhenDuplicateSequence()
    {
        var sut = new PlannedFormationResolver(_repository, []);
        var existingCoachLayoutId = CoachLayoutId.CreateNew();
        var identifiers = new List<PlannedFormationSnapshot> { Snapshot(existingCoachLayoutId), Snapshot(existingCoachLayoutId)};
        _repository.FindManyAsync(Arg.Any<IEnumerable<PlannedFormationSnapshot>>()).Returns([]);
        
        _ = await sut.ResolveManyAsync(identifiers);
        
        _repository.Received(1).Store(Arg.Any<PlannedFormation>());
    }
    
    [Fact]
    public async Task ResolveMany_ShouldAddToKnownList_WhenSequenceCreated()
    {
        var knownFormations = new List<PlannedFormation>();
        var sut = new PlannedFormationResolver(_repository, knownFormations);
        var existingCoachLayoutId = CoachLayoutId.CreateNew();
        var identifiers = new List<PlannedFormationSnapshot> { Snapshot(existingCoachLayoutId)};
        _repository.FindManyAsync(Arg.Any<IEnumerable<PlannedFormationSnapshot>>()).Returns([]);
        
        _ = await sut.ResolveManyAsync(identifiers);
        
        Assert.Single(knownFormations,  l => l.Matches(Snapshot(existingCoachLayoutId)));
    }
    
    [Fact]
    public async Task ResolveMany_ShouldAddToKnownList_WhenSequenceLoadedFromRepository()
    {
        var knownFormations = new List<PlannedFormation>();
        var sut = new PlannedFormationResolver(_repository, knownFormations);
        var existingCoachLayoutId = CoachLayoutId.CreateNew();
        var existingPlannedFormation = ExistingFormation(existingCoachLayoutId);
        var identifiers = new List<PlannedFormationSnapshot> { Snapshot(existingCoachLayoutId)};
        _repository.FindManyAsync(Arg.Any<IEnumerable<PlannedFormationSnapshot>>()).Returns([existingPlannedFormation]);
        
        _ = await sut.ResolveManyAsync(identifiers);
        
        Assert.Single(knownFormations,  existingPlannedFormation);
    }
}