using DBetter.TrainCompositions.Domain.PlannedFormations;
using Microsoft.Extensions.DependencyInjection;

namespace DBetter.TrainCompositions.Infrastructure.PlannedFormations;

public static class DependencyInjection
{
    public static void AddPlannedFormations(this IServiceCollection services)
    {
        services.AddScoped<IPlannedFormationRepository, PlannedFormationRepository>();
    }
}