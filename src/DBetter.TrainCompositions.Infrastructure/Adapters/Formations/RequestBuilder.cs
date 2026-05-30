using DBetter.TrainCompositions.Infrastructure.Adapters.Formations.Dtos;
using DBetter.TrainCompositions.Infrastructure.BahnDe;

namespace DBetter.TrainCompositions.Infrastructure.TrainRuns;

public class RequestBuilder(int serviceNumber, string originStation, DateTime departureTime, string destinationStation, DateTime arrivalTime)
{
    public RootParameters Build()
    {
        return new RootParameters
        {
            Buchungskontext = new()
            {
                BuchungsKontextDaten = new BuchungsKontextDaten
                {
                    Zugnummer = serviceNumber.ToString(),
                    AbfahrtHalt = new StartHalt
                    {
                        LocationId = originStation,
                        AbfahrtZeit = departureTime.ToBahnTime()
                    },
                    AnkunftHalt = new EndHalt
                    {
                        LocationId = destinationStation,
                        AnkunftZeit = arrivalTime.ToBahnTime()
                    }
                }
            }
        };
    }
    
}