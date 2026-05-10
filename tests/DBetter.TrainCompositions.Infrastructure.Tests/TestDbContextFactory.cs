using DBetter.TrainCompositions.Infrastructure.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace DBetter.TrainCompositions.Infrastructure.Tests;

public static class TestDbContextFactory
{
    public static DBetterContext Create(string connectionString)
    {
        var options = new DbContextOptionsBuilder<DBetterContext>()
            .UseNpgsql(connectionString)
            .Options;

        var db = new DBetterContext(options);
        db.Database.Migrate();
        return db;
    }
}