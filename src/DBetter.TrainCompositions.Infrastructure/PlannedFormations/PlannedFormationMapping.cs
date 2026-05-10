using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DBetter.TrainCompositions.Infrastructure.PlannedFormations;

public class PlannedFormationMapping: IEntityTypeConfiguration<PlannedFormationPersistenceDto>
{
    public void Configure(EntityTypeBuilder<PlannedFormationPersistenceDto> builder)
    {
        builder.ToTable("PlannedFormations");
        
        builder.HasKey(plannedFormation => plannedFormation.Id);
        
        builder.HasIndex(plannedFormation => plannedFormation.CoachSequenceHash)
            .IsUnique();
        
        builder.HasIndex(plannedFormation => plannedFormation.ReverseCoachSequenceHash)
            .IsUnique();

        builder.OwnsMany(plannedFormation => plannedFormation.CoachSequence, coachBuilder =>
        {
            coachBuilder.ToTable("PlannedCoaches");
            
            coachBuilder.WithOwner().HasForeignKey("PlannedFormationId");

            coachBuilder.HasIndex("PlannedFormationId", nameof(PlannedCoachPersistenceDto.Position));
        });
    }
}