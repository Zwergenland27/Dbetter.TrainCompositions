using DBetter.TrainCompositions.Infrastructure.OutboxPattern;
using Microsoft.EntityFrameworkCore;

namespace DBetter.TrainCompositions.Infrastructure.PostgreSQL;

/// <summary>
/// Database context for
/// </summary>
public class DBetterContext(DbContextOptions<DBetterContext> options): DbContext(options)
{
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DBetterContext).Assembly);
    }
}