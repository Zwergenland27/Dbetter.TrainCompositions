using System.Net;
using System.Text.Json;
using System.Web;
using DBetter.TrainCompositions.Application.Ports.Formations;
using DBetter.TrainCompositions.Domain.CoachLayouts.ValueObjects;
using DBetter.TrainCompositions.Infrastructure.TrainRuns;
using HtmlAgilityPack;

namespace DBetter.TrainCompositions.Infrastructure.Adapters.Formations;

public class BahnDeSeatingPlanProvider(HttpClient http): IPlannedFormationProvider
{
    public async Task<List<PlannedVehicleDto>?> GetForSectionAsync(int serviceNumber, string originStationEva, DateTime departureTime, string destinationStationEva, DateTime arrivalTime)
    {
        var request = new RequestBuilder(serviceNumber, originStationEva, departureTime, destinationStationEva, arrivalTime).Build();
        var requestJson = JsonSerializer.Serialize(request, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        
        var uri = HttpUtility.UrlPathEncode(requestJson);
        var response = await http.GetAsync($"gsd/gsd_v3?data={uri}");
        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.Conflict) return null;
        }
        
        var html = await response.Content.ReadAsStringAsync();
        var document = new HtmlDocument();
        document.LoadHtml(html);
        var scriptNode = document.DocumentNode.SelectSingleNode("//script[@type='application/json' and @id='ssr_data']");
        if (scriptNode is null) return null;
        
        var json = scriptNode.InnerText;
        var vehicleSequence = JsonSerializer.Deserialize<PlannedSequence>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (vehicleSequence is null) return null;

        return vehicleSequence.Zugfahrt.Zugteile.Select(g => new PlannedVehicleDto
        {
            CoachIdentifiers = g.Wagen.Select(v => CoachLayoutIdentifier.Create(v.Wagentyp)).ToList(),
        }).ToList();
    }
}