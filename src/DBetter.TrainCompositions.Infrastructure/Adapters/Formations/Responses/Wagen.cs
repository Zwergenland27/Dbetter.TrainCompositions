namespace DBetter.TrainCompositions.Infrastructure.TrainRuns;

public class Wagen
{
    public required List<Platz> Plaetze { get; set; }
    
    public required string Nummer  { get; set; }
    
    public required string Wagentyp { get; set; }
}