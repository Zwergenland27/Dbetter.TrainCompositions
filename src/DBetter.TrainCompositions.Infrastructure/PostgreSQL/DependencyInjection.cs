using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DBetter.TrainCompositions.Infrastructure.PostgreSQL;

public static class DependencyInjection
{
    /// <summary>
    /// Adds postgreSQL database
    /// </summary>
    public static void AddPostgreSql(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = new PostgreSqlSettings();
        configuration.Bind(PostgreSqlSettings.SectionName, settings);
        settings.Validate();
        
        services.AddDbContext<DBetterContext>(options =>
        {
            options.UseNpgsql(settings.ConnectionString);
        });
    }
}