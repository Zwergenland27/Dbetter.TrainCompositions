using System.Text.Json;
using CleanDomainValidation.Domain;
using DBetter.TrainCompositions.Application.Ports.TrainRuns;
using DBetter.TrainCompositions.Domain.Shared.Stations;
using DBetter.TrainCompositions.Domain.Shared.TrainRuns;

namespace DBetter.TrainCompositions.Infrastructure.Adapters.TrainRuns;

public class DBetterTrainRunProvider(HttpClient http): ITrainRunProvider
{
    public async Task<CanFail<TrainRunRouteDto>> GetRouteAsync(ExternalTrainRunId id)
    {
        http.DefaultRequestHeaders.Accept.Clear();
        http.DefaultRequestHeaders.Add("Accept", "application/dbetter.route-only+json");
        
        var result = await http.GetAsync($"train-runs/{id.Value}");
        var content = await result.Content.ReadAsStringAsync();
        if (!result.IsSuccessStatusCode)
        {
            return TrainRunErrors.Unknown;
        }
        
        var trainRun = JsonSerializer.Deserialize<TrainRunResponse>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        
        if (trainRun is null) return TrainRunErrors.Unknown;

        return new TrainRunRouteDto
        {
            ServiceNumber = trainRun.ServiceNumber,
            Stops = trainRun.Stops.Select(s => new TrainRunStopDto
                {
                    Id = ExternalStationId.Create(s.Id).Value,
                    EvaNumber = s.EvaNumber,
                    PlannedDepartureTime = s.DepartureTime?.Planned,
                    PlannedArrivalTime = s.ArrivalTime?.Planned,
                    RouteIndex = s.RouteIndex
                })
                .ToList()
        };
    }
}