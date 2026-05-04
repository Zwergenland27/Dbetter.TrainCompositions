using DBetter.TrainCompositions.Domain.CoachLayouts;
using Microsoft.Extensions.DependencyInjection;

namespace DBetter.TrainCompositions.Infrastructure.CoachLayouts;

public static class DependencyInjection
{
    public static void AddCoachLayouts(this IServiceCollection services)
    {
        services.AddScoped<ICoachLayoutRepository, CoachLayoutRepository>();
    }
}