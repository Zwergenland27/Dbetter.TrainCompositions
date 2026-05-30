namespace DBetter.TrainCompositions.Infrastructure.BahnDe;

public static class DateTimeFactory
{
    public static string ToBahnTime(this DateTime date)
    {
        TimeZoneInfo germanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin");
        return TimeZoneInfo.ConvertTimeFromUtc(date, germanTimeZone).ToString("yyyy-MM-ddTHH:mm:ss");
    }
}