using DBetter.TrainCompositions.Infrastructure.CoachLayouts;
using DBetter.TrainCompositions.Infrastructure.OutboxPattern;
using DBetter.TrainCompositions.Infrastructure.PlannedFormations;
using Microsoft.EntityFrameworkCore;

namespace DBetter.TrainCompositions.Infrastructure.PostgreSQL;

/// <summary>
/// Database context for
/// </summary>
public class DBetterContext(DbContextOptions<DBetterContext> options): DbContext(options)
{
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    
    public DbSet<CoachLayoutPersistenceDto> CoachLayouts { get; set; }
    
    public DbSet<PlannedFormationPersistenceDto> PlannedFormations { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DBetterContext).Assembly);
    }
}