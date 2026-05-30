using System.Net;
using DBetter.TrainCompositions.Application.Ports.Formations;
using DBetter.TrainCompositions.Application.Ports.TrainRuns;
using DBetter.TrainCompositions.Infrastructure.Adapters.Formations;
using DBetter.TrainCompositions.Infrastructure.Adapters.TrainRuns;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DBetter.TrainCompositions.Infrastructure.Adapters;

public static class DependencyInjection
{
    public static void AddAdapters(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IPlannedFormationProvider, BahnDeSeatingPlanProvider>(client =>
                client.BaseAddress = new Uri("https://www.bahn.de/web/api/"))
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip
            });

        services.AddHttpClient<ITrainRunProvider, DBetterTrainRunProvider>(client =>
            client.BaseAddress = new Uri("https://api.dbetter.de/"));
    }
}