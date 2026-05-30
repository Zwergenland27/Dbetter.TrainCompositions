using DBetter.TrainCompositions.Infrastructure.TrainRuns;

namespace DBetter.TrainCompositions.Infrastructure.Adapters.Formations.Dtos;

public class BuchungsKontextDaten
{
    public required string Zugnummer { get; init; }
    public required StartHalt AbfahrtHalt { get; init; }
    public required EndHalt AnkunftHalt { get; init; }

    public Platzbedarf[] Platzbedarfe { get; } = [new ()];
}

public class Platzbedarf
{
    public string PlatzprofilCode { get; } = "StandardEinzelperson";
    public int Anzahl { get; } = 1;
    public string Klasse { get; } = "KLASSE_2";
}