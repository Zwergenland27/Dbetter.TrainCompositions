using DBetter.TrainCompositions.Domain.CoachLayouts;
using DBetter.TrainCompositions.Domain.CoachLayouts.ValueObjects;
using DBetter.TrainCompositions.Infrastructure.PostgreSQL;

namespace DBetter.TrainCompositions.Infrastructure.CoachLayouts;

public class CoachLayoutPersistenceDto: IPersistenceDto<CoachLayout, CoachLayoutPersistenceDto>
{
    public required Guid Id { get; init; }
    
    public required string Identifier { get; set; }
    
    public required string? ConstructionType { get; set; }
    
    public static CoachLayoutPersistenceDto FromDomain(CoachLayout domain)
    {
        return new CoachLayoutPersistenceDto
        {
            Id = domain.Id.Value,
            Identifier = domain.Identifier.Value,
            ConstructionType = domain.ConstructionType?.Value,
        };
    }

    public CoachLayout ToDomain()
    {
        return new CoachLayout(
            new CoachLayoutId(Id),
            new  CoachLayoutIdentifier(Identifier),
            ConstructionType is null ? null : new ConstructionType(ConstructionType),
            new Amenities());
    }

    public void Apply(CoachLayout domain)
    {
        Identifier = domain.Identifier.Value;
        ConstructionType = domain.ConstructionType?.Value;
    }
}