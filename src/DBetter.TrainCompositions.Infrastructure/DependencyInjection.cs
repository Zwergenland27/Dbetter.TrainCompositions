using CleanMediator;
using DBetter.TrainCompositions.Application.Abstractions;
using DBetter.TrainCompositions.Infrastructure.PostgreSQL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DBetter.TrainCompositions.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCleanMediator(options =>
        {
            options.RegisterServicesFromAssembly(typeof(IUnitOfWork).Assembly);
        });
        
        services.AddPostgreSql(configuration);
    }
}