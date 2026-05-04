using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DBetter.TrainCompositions.Infrastructure.CoachLayouts;

public class CoachLayoutMapping: IEntityTypeConfiguration<CoachLayoutPersistenceDto>
{
    public void Configure(EntityTypeBuilder<CoachLayoutPersistenceDto> builder)
    {
        builder.ToTable("CoachLayouts");
        
        builder.HasKey(x => x.Id);
        
        builder.HasIndex(x => x.Identifier)
            .IsUnique();
    }
}