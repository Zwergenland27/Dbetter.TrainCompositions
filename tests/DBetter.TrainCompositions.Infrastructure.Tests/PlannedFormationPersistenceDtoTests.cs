using System.Xml;
using DBetter.TrainCompositions.Domain.CoachLayouts.ValueObjects;
using DBetter.TrainCompositions.Domain.PlannedFormations;
using DBetter.TrainCompositions.Domain.PlannedFormations.Coaches;
using DBetter.TrainCompositions.Domain.PlannedFormations.Coaches.ValueObjects;
using DBetter.TrainCompositions.Domain.PlannedFormations.ValueObjects;
using DBetter.TrainCompositions.Infrastructure.PlannedFormations;

namespace DBetter.TrainCompositions.Infrastructure.Tests;

public class PlannedFormationPersistenceDtoTests
{
    private static PlannedCoach MakeCoach(short position, Guid? layoutId = null) =>
        new(new PlannedCoachPosition(position), layoutId is null ? CoachLayoutId.CreateNew() : new CoachLayoutId(layoutId.Value));
 
    private static PlannedFormation MakeFormation(params PlannedCoach[] coaches)
    {
        var id = PlannedFormationId.CreateNew();
        return new PlannedFormation(id, coaches.ToList());
    }

    [Fact]
    public void FromDomain_ShouldMapId()
    {
        var domain = new PlannedFormation(
            PlannedFormationId.CreateNew(),
            [MakeCoach(1)]);
 
        var dto = PlannedFormationPersistenceDto.FromDomain(domain);
        
        Assert.Equal(domain.Id.Value, dto.Id);
    }
 
    [Fact]
    public void FromDomain_ShouldMapCoachSequenceCount()
    {
        var domain = MakeFormation(MakeCoach(1), MakeCoach(2), MakeCoach(3));
 
        var dto = PlannedFormationPersistenceDto.FromDomain(domain);
        
        Assert.Equal(dto.CoachSequence.Count, domain.CoachSequence.Count);
    }
 
    [Fact]
    public void FromDomain_ShouldMapCoachPositionsInOrder()
    {
        var domain = MakeFormation(MakeCoach(1), MakeCoach(2), MakeCoach(3));
 
        var dto = PlannedFormationPersistenceDto.FromDomain(domain);
 
        Assert.Equal(domain.CoachSequence.Select(c => c.Id.Value), dto.CoachSequence.Select(c => c.Position));
    }

    [Fact]
    public void FromDomain_ShouldSetCoachSequenceHash()
    {
        var layoutId = CoachLayoutId.CreateNew();
        var domain = MakeFormation(MakeCoach(1, layoutId.Value));
 
        var dto = PlannedFormationPersistenceDto.FromDomain(domain);
 
        var expected = PlannedFormationPersistenceDto.ComputeCoachSequenceHash(
            [layoutId]);
        
        Assert.Equal(expected, dto.CoachSequenceHash);
    }
 
    [Fact]
    public void FromDomain_ShouldSetReverseCoachSequenceHash()
    {
        var id1 = CoachLayoutId.CreateNew();
        var id2 = CoachLayoutId.CreateNew();
        var domain = MakeFormation(MakeCoach(1, id1.Value), MakeCoach(2, id2.Value));
 
        var dto = PlannedFormationPersistenceDto.FromDomain(domain);
 
        var expectedReverse = PlannedFormationPersistenceDto.ComputeCoachSequenceHash(
            [id2, id1]);
 
        Assert.Equal(expectedReverse, dto.ReverseCoachSequenceHash);
    }
    
    [Fact]
    public void ToDomain_ShouldMapId()
    {
        var dto = new PlannedFormationPersistenceDto
        {
            Id = PlannedFormationId.CreateNew().Value,
            CoachSequence = [],
            CoachSequenceHash = "",
            ReverseCoachSequenceHash = "",
        };
 
        var domain = dto.ToDomain();
 
        Assert.Equal(dto.Id, domain.Id.Value);
    }
    
    [Fact]
    public void ToDomain_ShouldMapAllCoaches()
    {
        var id1 = CoachLayoutId.CreateNew();
        var id2 = CoachLayoutId.CreateNew();
        var dto = new PlannedFormationPersistenceDto
        {
            Id = PlannedFormationId.CreateNew().Value,
            CoachSequence =
            [
                new PlannedCoachPersistenceDto { Position = 1, LayoutId = id1.Value },
                new PlannedCoachPersistenceDto { Position = 2, LayoutId = id2.Value },
            ],
            CoachSequenceHash = "",
            ReverseCoachSequenceHash = "",
        };
 
        var domain = dto.ToDomain();
 
        Assert.Equal(2, domain.CoachSequence.Count);
        Assert.Equal([id1, id2], domain.CoachSequence.Select(c => c.LayoutId));
    }
    
    [Fact]
    public void ComputeCoachSequenceHash_ShouldReturn32LowercaseHexChars_WhenInputIsNonEmpty()
    {
        var hash = PlannedFormationPersistenceDto.ComputeCoachSequenceHash(
            [CoachLayoutId.CreateNew()]);
 
        Assert.Matches("^[0-9a-f]{32}$", hash);
    }
 
    [Fact]
    public void ComputeCoachSequenceHash_ShouldReturnConsistentResult_WhenCalledTwice()
    {
        var ids = new[] { CoachLayoutId.CreateNew() };
 
        var h1 = PlannedFormationPersistenceDto.ComputeCoachSequenceHash(ids);
        var h2 = PlannedFormationPersistenceDto.ComputeCoachSequenceHash(ids);
 
        Assert.Equal(h1, h2);
    }
 
    [Fact]
    public void ComputeCoachSequenceHash_ShouldProduceDifferentResult_WhenSingleIdDiffers()
    {
        var id1 = CoachLayoutId.CreateNew();
        var id2 = CoachLayoutId.CreateNew();
 
        var h1 = PlannedFormationPersistenceDto.ComputeCoachSequenceHash([id1]);
        var h2 = PlannedFormationPersistenceDto.ComputeCoachSequenceHash([id2]);
 
        Assert.NotEqual(h1, h2);
    }
 
    [Fact]
    public void ComputeCoachSequenceHash_ShouldProduceDifferentResult_WhenOrderChanges()
    {
        var id1 = CoachLayoutId.CreateNew();
        var id2 = CoachLayoutId.CreateNew();
 
        var h1 = PlannedFormationPersistenceDto.ComputeCoachSequenceHash(
            [id1, id2]);
        var h2 = PlannedFormationPersistenceDto.ComputeCoachSequenceHash(
            [id2, id1]);
 
        Assert.NotEqual(h1, h2);
    }
 
    [Fact]
    public void ComputeCoachSequenceHash_ShouldProduceDifferentResult_WhenLengthDiffers()
    {
        var layoutId = CoachLayoutId.CreateNew();
 
        var h1 = PlannedFormationPersistenceDto.ComputeCoachSequenceHash([layoutId]);
        var h2 = PlannedFormationPersistenceDto.ComputeCoachSequenceHash([layoutId, layoutId]);
 
        Assert.NotEqual(h1, h2);
    }
 
    [Fact]
    public void ComputeCoachSequenceHash_ShouldProduceSameResult_WhenIdsAreIdentical()
    {
        var id1 = CoachLayoutId.CreateNew();
        var id2 = CoachLayoutId.CreateNew();
        var ids = new[] { id1, id2 };
 
        Assert.Equal(
            PlannedFormationPersistenceDto.ComputeCoachSequenceHash(ids),
            PlannedFormationPersistenceDto.ComputeCoachSequenceHash(ids));
    }
}