namespace DBetter.TrainCompositions.Infrastructure.PostgreSQL;

public class PostgreSqlSettings
{
    /// <summary>
    /// Section name in appsettings
    /// </summary>
    public const string SectionName = "PostgreSQL";

    /// <summary>
    /// Connection string to productive database
    /// </summary>
    public string ConnectionString { get; set; } = null!;

    public void Validate()
    {
        if(ConnectionString is null)
            throw new ArgumentNullException(nameof(ConnectionString), $"{SectionName}__{nameof(ConnectionString)} must be set");
    }
}