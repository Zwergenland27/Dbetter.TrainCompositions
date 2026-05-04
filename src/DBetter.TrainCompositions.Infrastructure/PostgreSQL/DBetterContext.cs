using Microsoft.EntityFrameworkCore;

namespace DBetter.TrainCompositions.Infrastructure.PostgreSQL;

/// <summary>
/// Database context for
/// </summary>
public class DBetterContext(DbContextOptions<DBetterContext> options): DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DBetterContext).Assembly);
    }
}