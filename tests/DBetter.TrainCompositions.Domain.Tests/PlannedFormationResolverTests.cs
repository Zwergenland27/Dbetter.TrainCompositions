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
    private readonly PlannedFormationResolver _sut;

    public PlannedFormationResolverTests()
    {
        _repository = Substitute.For<IPlannedFormationRepository>();
        _sut = new PlannedFormationResolver(_repository);
    }

    // --- Helpers ---

    private static PlannedFormationSnapshot Snapshot(params CoachLayoutId[] coaches)
        => new(coaches.ToList());

    private static PlannedFormation ExistingFormation(params CoachLayoutId[] coaches)
    {
        var positions = coaches
            .Select((id, i) => new PlannedCoach(new PlannedCoachPosition((short)(i + 1)), id))
            .ToList();
        return new PlannedFormation(PlannedFormationId.CreateNew(), positions);
    }

    // --- Tests ---

    [Fact]
    public async Task ResolveManyAsync_ShouldReturnEmpty_WhenInputIsEmpty()
    {
        _repository.FindManyAsync(Arg.Any<List<PlannedFormationSnapshot>>()).Returns([]);

        var result = await _sut.ResolveManyAsync([]);
        
        Assert.Empty(result);
    }

    [Fact]
    public async Task ResolveManyAsync_ShouldReturnExisting_WhenAllSequencesAlreadyExist()
    {
        var coachId = CoachLayoutId.CreateNew();
        var snapshot = Snapshot(coachId);
        var existing = ExistingFormation(coachId);
        var snapshots = new List<PlannedFormationSnapshot>{snapshot};

        _repository.FindManyAsync(snapshots).Returns([existing]);

        var result = await _sut.ResolveManyAsync(snapshots);
        
        Assert.Single(result, existing);
        _repository.DidNotReceive().Store(Arg.Any<PlannedFormation>());
    }

    [Fact]
    public async Task ResolveManyAsync_ShouldCreateAndStore_WhenSequenceDoesNotExist()
    {
        var snapshot = Snapshot(CoachLayoutId.CreateNew());
        var snapshots = new List<PlannedFormationSnapshot>{snapshot};

        _repository.FindManyAsync(snapshots).Returns([]);

        var result = await _sut.ResolveManyAsync(snapshots);
        
        Assert.Single(result);
        _repository.Received(1).Store(Arg.Any<PlannedFormation>());
    }

    [Fact]
    public async Task ResolveManyAsync_ShouldCreateOnlyMissing_WhenSomeSequencesAlreadyExist()
    {
        var existingCoachId = CoachLayoutId.CreateNew();
        var newCoachId = CoachLayoutId.CreateNew();

        var existingSnapshot = Snapshot(existingCoachId);
        var newSnapshot = Snapshot(newCoachId);
        var existing = ExistingFormation(existingCoachId);
        var snapshots = new List<PlannedFormationSnapshot>{existingSnapshot, newSnapshot};

        _repository.FindManyAsync(Arg.Any<List<PlannedFormationSnapshot>>()).Returns([existing]);

        var result = await _sut.ResolveManyAsync(snapshots);
        
        Assert.Equal(2, result.Count);
        _repository.Received(1).Store(Arg.Any<PlannedFormation>());
    }

    [Fact]
    public async Task ResolveManyAsync_ShouldReturnAllCreated_WhenMultipleNewSequencesGiven()
    {
        var snapshots = Enumerable.Range(0, 3)
            .Select(_ => Snapshot(CoachLayoutId.CreateNew()))
            .ToList();

        _repository.FindManyAsync(snapshots).Returns([]);

        var result = await _sut.ResolveManyAsync(snapshots);
        
        Assert.Equal(3, result.Count);
        _repository.Received(3).Store(Arg.Any<PlannedFormation>());
    }
}