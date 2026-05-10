using DBetter.TrainCompositions.Domain.CoachLayouts.ValueObjects;
using DBetter.TrainCompositions.Domain.PlannedFormations.Coaches;
using DBetter.TrainCompositions.Domain.PlannedFormations.Coaches.ValueObjects;
using DBetter.TrainCompositions.Infrastructure.PlannedFormations;

namespace DBetter.TrainCompositions.Infrastructure.Tests;

public class PlannedCoachPersistenceDtoTests
{
    [Fact]
    public void FromDomain_ShouldMapPosition()
    {
        var domain = new PlannedCoach(
            new PlannedCoachPosition(3),
            CoachLayoutId.CreateNew());
 
        var dto = PlannedCoachPersistenceDto.FromDomain(domain);
 
        Assert.Equal(domain.LayoutId.Value, dto.LayoutId);
    }
 
    [Fact]
    public void FromDomain_ShouldMapLayoutId()
    {
        var domain = new PlannedCoach(
            new PlannedCoachPosition(1),
            CoachLayoutId.CreateNew());
 
        var dto = PlannedCoachPersistenceDto.FromDomain(domain);
 
        Assert.Equal(domain.LayoutId.Value, dto.LayoutId);
    }
    
    [Fact]
    public void ToDomain_ShouldMapPosition()
    {
        var dto = new PlannedCoachPersistenceDto
        {
            Position = 5,
            LayoutId = CoachLayoutId.CreateNew().Value
        };
 
        var domain = dto.ToDomain();
        
        Assert.Equal(dto.Position, domain.Id.Value);
    }
 
    [Fact]
    public void ToDomain_ShouldMapLayoutId()
    {
        var dto = new PlannedCoachPersistenceDto
        {
            Position = 1,
            LayoutId = CoachLayoutId.CreateNew().Value,
        };
 
        var domain = dto.ToDomain();
 
        Assert.Equal(dto.LayoutId, domain.LayoutId.Value);
    }
    
    [Fact]
    public void Apply_ShouldUpdateLayoutId_WhenDomainHasNewLayoutId()
    {
        var dto = new PlannedCoachPersistenceDto
        {
            Position = 1,
            LayoutId = CoachLayoutId.CreateNew().Value,
        };
        
        var updatedDomain = new PlannedCoach(
            new PlannedCoachPosition(1),
            CoachLayoutId.CreateNew());
 
        dto.Apply(updatedDomain);
        
        Assert.Equal(updatedDomain.LayoutId.Value, dto.LayoutId);
    }
}