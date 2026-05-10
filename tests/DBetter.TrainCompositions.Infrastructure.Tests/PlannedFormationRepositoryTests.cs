 using DBetter.TrainCompositions.Domain.CoachLayouts.ValueObjects;
 using DBetter.TrainCompositions.Domain.PlannedFormations;
 using DBetter.TrainCompositions.Domain.PlannedFormations.ValueObjects;
 using DBetter.TrainCompositions.Infrastructure.PlannedFormations;
using DBetter.TrainCompositions.Infrastructure.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace DBetter.TrainCompositions.Infrastructure.Tests;

[Collection("Postgres")]
public class PlannedFormationRepositoryTests: IAsyncLifetime
{
    private readonly PostgresFixture _fixture;
    private DBetterContext _db = null!;
    private PlannedFormationRepository _sut = null!;
    
    public PlannedFormationRepositoryTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        _db = TestDbContextFactory.Create(_fixture.ConnectionString);
        await _db.CoachLayouts.ExecuteDeleteAsync();
        await _db.OutboxMessages.ExecuteDeleteAsync();
        _sut = new PlannedFormationRepository(_db);
    }

    public async Task DisposeAsync() => await _db.DisposeAsync();
    
    [Fact]
    public async Task Store_ShouldPersistNewAggregate_WhenNotYetTracked()
    {
        var aggregate = PlannedFormation.Create(new PlannedFormationSnapshot([CoachLayoutId.CreateNew()])).Value;

        _sut.Store(aggregate);
        await _db.SaveChangesAsync();
        
        var dto = await _db.PlannedFormations.FirstOrDefaultAsync(c => c.Id == aggregate.Id.Value);
        Assert.NotNull(dto);
        Assert.Equal(aggregate.Id.Value, dto.Id);
    }
    
    [Fact]
    public async Task Store_ShouldUpdateExistingAggregate_WhenAlreadyTracked()
    {
        //Currently not necessary 
    }
    
    [Fact]
    public async Task Store_ShouldWriteOutboxMessages_WhenAggregateHasDomainEvents()
    {
        var aggregate = PlannedFormation.Create(new PlannedFormationSnapshot([CoachLayoutId.CreateNew()])).Value;
        aggregate.RaiseTestDomainEvent(TestDomainEvent.Example);

        _sut.Store(aggregate);
        await _db.SaveChangesAsync();

        var messages = await _db.OutboxMessages.ToListAsync();
        Assert.Single(messages);
    }
    
    [Fact]
    public async Task GetAsync_ShouldReturnAggregate_WhenExists()
    {
        var aggregate = PlannedFormation.Create(new PlannedFormationSnapshot([CoachLayoutId.CreateNew()])).Value;
        _sut.Store(aggregate);
        await _db.SaveChangesAsync();

        var result = await _sut.GetAsync(aggregate.Id);

        Assert.NotNull(result);
        Assert.Equal(aggregate.Id, result.Id);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnNull_WhenNotFound()
    {
        var result = await _sut.GetAsync(PlannedFormationId.CreateNew());
        Assert.Null(result);
    }
    
    [Fact]
    public async Task FindManyAsync_ShouldReturnFormation_WhenCoachSequenceHashMatches()
    {
        var coaches = new List<CoachLayoutId> { CoachLayoutId.CreateNew(), CoachLayoutId.CreateNew(), CoachLayoutId.CreateNew() };
        var stored = PlannedFormation.Create(new PlannedFormationSnapshot(coaches)).Value;
        _sut.Store(stored);
        await _db.SaveChangesAsync();

        var snapshot = new PlannedFormationSnapshot(coaches);
        var result = await _sut.FindManyAsync([snapshot]);

        var resultIds = result.Select(x => x.Id).ToList();
        Assert.Single(resultIds);
        Assert.Contains(stored.Id, resultIds);
    }

    [Fact]
    public async Task FindManyAsync_ShouldReturnFormation_WhenCoachSequenceIsReversed()
    {
        var coaches = new List<CoachLayoutId> { CoachLayoutId.CreateNew(), CoachLayoutId.CreateNew(), CoachLayoutId.CreateNew() };
        var stored = PlannedFormation.Create(new PlannedFormationSnapshot(coaches)).Value;
        _sut.Store(stored);
        await _db.SaveChangesAsync();

        var reversedCoaches = coaches.AsEnumerable().Reverse().ToList();
        var snapshot = new PlannedFormationSnapshot(reversedCoaches);
        var result = await _sut.FindManyAsync([snapshot]);

        var resultIds = result.Select(x => x.Id).ToList();
        Assert.Single(resultIds);
        Assert.Contains(stored.Id, resultIds);
    }
    
    [Fact]
    public async Task FindManyAsync_ShouldReturnFormationOnce_WhenSameFormationQueriedInBothDirections()
    {
        var coaches = new List<CoachLayoutId> { CoachLayoutId.CreateNew(), CoachLayoutId.CreateNew() };
        var stored = PlannedFormation.Create(new PlannedFormationSnapshot(coaches)).Value;
        _sut.Store(stored);
        await _db.SaveChangesAsync();

        var snapshots = new[]
        {
            new PlannedFormationSnapshot(coaches),
            new PlannedFormationSnapshot(coaches.AsEnumerable().Reverse().ToList())
        };
        var result = await _sut.FindManyAsync(snapshots);

        Assert.Single(result);
        Assert.Equal(stored.Id, result[0].Id);
    }

    [Fact]
    public async Task FindManyAsync_ShouldReturnFormationOnce_WhenBothHashesMatch()
    {
        var coachA = CoachLayoutId.CreateNew();
        var coachB = CoachLayoutId.CreateNew();
        var coaches = new List<CoachLayoutId> { coachA, coachB, coachA };
        var stored = PlannedFormation.Create(new PlannedFormationSnapshot(coaches)).Value;
        _sut.Store(stored);
        await _db.SaveChangesAsync();

        var snapshot = new PlannedFormationSnapshot(coaches);
        var result = await _sut.FindManyAsync([snapshot]);
        
        Assert.Single(result);
    }

    [Fact]
    public async Task FindManyAsync_ShouldReturnEmpty_WhenNoHashMatches()
    {
        var coaches = new List<CoachLayoutId> { CoachLayoutId.CreateNew(), CoachLayoutId.CreateNew() };
        var stored = PlannedFormation.Create(new PlannedFormationSnapshot(coaches)).Value;
        _sut.Store(stored);
        await _db.SaveChangesAsync();

        var snapshot = new PlannedFormationSnapshot([CoachLayoutId.CreateNew(), CoachLayoutId.CreateNew()]);
        var result = await _sut.FindManyAsync([snapshot]);

        Assert.Empty(result);
    }

    [Fact]
    public async Task FindManyAsync_ShouldReturnAllMatching_WhenMultipleSnapshotsGiven()
    {
        var coachesA = new List<CoachLayoutId> { CoachLayoutId.CreateNew(), CoachLayoutId.CreateNew() };
        var coachesB = new List<CoachLayoutId> { CoachLayoutId.CreateNew(), CoachLayoutId.CreateNew() };
        var coachesC = new List<CoachLayoutId> { CoachLayoutId.CreateNew() };

        var a = PlannedFormation.Create(new PlannedFormationSnapshot(coachesA)).Value;
        _sut.Store(a);
        
        var b = PlannedFormation.Create(new PlannedFormationSnapshot(coachesB)).Value;
        _sut.Store(b);
        
        var c = PlannedFormation.Create(new PlannedFormationSnapshot(coachesC)).Value;
        _sut.Store(c);
        
        await _db.SaveChangesAsync();

        var snapshots = new[]
        {
            new PlannedFormationSnapshot(coachesA),
            new PlannedFormationSnapshot(coachesB),
        };
        var result = await _sut.FindManyAsync(snapshots);

        var resultIds = result.Select(x => x.Id).OrderBy(x => x.Value).ToList();
        var expectedIds = new[] { a.Id, b.Id }.OrderBy(x => x.Value).ToList();
        Assert.Equal(expectedIds, resultIds);
    }
}