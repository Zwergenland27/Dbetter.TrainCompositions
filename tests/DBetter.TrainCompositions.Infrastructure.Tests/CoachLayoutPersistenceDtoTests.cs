using DBetter.TrainCompositions.Domain.CoachLayouts;
using DBetter.TrainCompositions.Domain.CoachLayouts.ValueObjects;
using DBetter.TrainCompositions.Infrastructure.CoachLayouts;

namespace DBetter.TrainCompositions.Infrastructure.Tests;

public class CoachLayoutPersistenceDtoTests
{
    [Fact]
    public void FromDomain_ShouldMapAllProperties()
    {
        var id = Guid.NewGuid();
        var domain = new CoachLayout(
            new CoachLayoutId(id),
            new CoachLayoutIdentifier("I8082"),
            new ConstructionType("Bpmbz"),
            new Amenities());
        
        var dto = CoachLayoutPersistenceDto.FromDomain(domain);
        
        Assert.Equal(id, dto.Id);
        Assert.Equal("I8082", dto.Identifier);
        Assert.Equal("Bpmbz", dto.ConstructionType);
    }
    
    [Fact]
    public void FromDomain_ShouldMapNullConstructionType()
    {
        var domain = new CoachLayout(
            new CoachLayoutId(Guid.NewGuid()),
            new CoachLayoutIdentifier("I8082"),
            null,
            new Amenities());

        var dto = CoachLayoutPersistenceDto.FromDomain(domain);

        Assert.Null(dto.ConstructionType);
    }
    
    [Fact]
    public void ToDomain_ShouldMapAllProperties()
    {
        var id = Guid.NewGuid();
        var dto = new CoachLayoutPersistenceDto
        {
            Id = id,
            Identifier = "I8082",
            ConstructionType = "Bpmbz",
        };

        var domain = dto.ToDomain();

        Assert.Equal(id, domain.Id.Value);
        Assert.Equal("I8082", domain.Identifier.Value);
        Assert.Equal("Bpmbz", domain.ConstructionType!.Value);
    }
    
    
    [Fact]
    public void ToDomain_ShouldMapNullConstructionType()
    {
        var dto = new CoachLayoutPersistenceDto
        {
            Id = Guid.NewGuid(),
            Identifier = "I8082",
            ConstructionType = null,
        };

        var domain = dto.ToDomain();

        Assert.Null(domain.ConstructionType);
    }
    
    
    [Fact]
    public void Apply_ShouldUpdateMutableProperties()
    {
        var id = Guid.NewGuid();
        var dto = new CoachLayoutPersistenceDto
        {
            Id = id,
            Identifier = "I8082",
            ConstructionType = "Bpmbz",
        };
        var updated = new CoachLayout(
            new CoachLayoutId(id),
            new CoachLayoutIdentifier("I8084"),
            new ConstructionType("Bpz"),
            new Amenities());

        dto.Apply(updated);

        Assert.Equal("I8084", dto.Identifier);
        Assert.Equal("Bpz", dto.ConstructionType);
    }
    
    [Fact]
    public void Apply_ShouldSetNullConstructionType()
    {
        var dto = new CoachLayoutPersistenceDto
        {
            Id = Guid.NewGuid(),
            Identifier = "I8082",
            ConstructionType = "Bpmbz",
        };
        var updated = new CoachLayout(
            new CoachLayoutId(dto.Id),
            new CoachLayoutIdentifier("I8084"),
            null,
            new Amenities());

        dto.Apply(updated);

        Assert.Null(dto.ConstructionType);
    }
}