using DBetter.TrainCompositions.Domain.CoachLayouts;
using DBetter.TrainCompositions.Domain.CoachLayouts.ValueObjects;
using DBetter.TrainCompositions.Infrastructure.CoachLayouts;
using DBetter.TrainCompositions.Infrastructure.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace DBetter.TrainCompositions.Infrastructure.Tests;

[Collection("Postgres")]
public class CoachLayoutRepositoryTests: IAsyncLifetime
{
    private readonly PostgresFixture _fixture;
    private DBetterContext _db = null!;
    private CoachLayoutRepository _sut = null!;
    
    public CoachLayoutRepositoryTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        _db = TestDbContextFactory.Create(_fixture.ConnectionString);
        await _db.CoachLayouts.ExecuteDeleteAsync();
        await _db.OutboxMessages.ExecuteDeleteAsync();
        _sut = new CoachLayoutRepository(_db);
    }

    public async Task DisposeAsync() => await _db.DisposeAsync();
    
    [Fact]
    public async Task Store_ShouldPersistNewAggregate_WhenNotYetTracked()
    {
        var aggregate = CoachLayout.CreateFromPlanned(new CoachLayoutIdentifier("I4080"));

        _sut.Store(aggregate);
        await _db.SaveChangesAsync();
        
        var dto = await _db.CoachLayouts.FirstOrDefaultAsync(c => c.Id == aggregate.Id.Value);
        Assert.NotNull(dto);
        Assert.Equal(aggregate.Id.Value, dto.Id);
    }
    
    [Fact]
    public async Task Store_ShouldUpdateExistingAggregate_WhenAlreadyTracked()
    {
        var aggregate = CoachLayout.CreateFromPlanned(new CoachLayoutIdentifier("I4080"));
        _sut.Store(aggregate);
        await _db.SaveChangesAsync();
        
        var loadedAggregate = await _sut.GetAsync(aggregate.Id);
        Assert.NotNull(loadedAggregate);

        loadedAggregate.UpdateConstructionType(new ConstructionType("Bpmz"));
        _sut.Store(loadedAggregate);
        await _db.SaveChangesAsync();
        
        var dto = await _db.CoachLayouts.FirstOrDefaultAsync(c => c.Id == aggregate.Id.Value);
        Assert.NotNull(dto);
        Assert.Equal(loadedAggregate.ConstructionType!.Value, dto.ConstructionType);
    }
    
    [Fact]
    public async Task Store_ShouldWriteOutboxMessages_WhenAggregateHasDomainEvents()
    {
        var aggregate =  CoachLayout.CreateFromPlanned(new CoachLayoutIdentifier("I4080"));
        aggregate.RaiseTestDomainEvent(TestDomainEvent.Example);

        _sut.Store(aggregate);
        await _db.SaveChangesAsync();

        var messages = await _db.OutboxMessages.ToListAsync();
        Assert.Single(messages);
    }
    
    [Fact]
    public async Task GetAsync_ShouldReturnAggregate_WhenExists()
    {
        var aggregate = CoachLayout.CreateFromPlanned(new CoachLayoutIdentifier("I4080"));
        _sut.Store(aggregate);
        await _db.SaveChangesAsync();

        var result = await _sut.GetAsync(aggregate.Id);

        Assert.NotNull(result);
        Assert.Equal(aggregate.Id, result.Id);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnNull_WhenNotFound()
    {
        var result = await _sut.GetAsync(CoachLayoutId.CreateNew());
        Assert.Null(result);
    }
    
    [Fact]
    public async Task FindManyAsync_ShouldReturnMatchingAggregates_WhenIdentifiersExist()
    {
        var a = CoachLayout.CreateFromPlanned(new CoachLayoutIdentifier("I4080"));
        _sut.Store(a);
        
        var b = CoachLayout.CreateFromPlanned(new CoachLayoutIdentifier("I5080"));
        _sut.Store(b);
        
        var c = CoachLayout.CreateFromPlanned(new CoachLayoutIdentifier("I6080"));
        _sut.Store(c);
        
        await _db.SaveChangesAsync();

        var result = await _sut.FindManyAsync([a.Identifier, b.Identifier, new CoachLayoutIdentifier("NotExist")]);

        var resultIds = result.Select(x => x.Id).OrderBy(x => x.Value).ToList();
        var expectedIds = new[] { a.Id, b.Id }.OrderBy(x => x.Value).ToList();
        Assert.Equal(expectedIds, resultIds);
    }
    
    [Fact]
    public async Task FindManyAsync_ShouldReturnEmptyList_WhenNoIdentifiersProvided()
    {
        var result = await _sut.FindManyAsync([]);
        Assert.Empty(result);
    }

    [Fact]
    public async Task FindManyAsync_ShouldReturnEmpty_WhenNoIdentifiersMatch()
    {
        var result = await _sut.FindManyAsync([new CoachLayoutIdentifier("I4080")]);
        Assert.Empty(result);
    }
}