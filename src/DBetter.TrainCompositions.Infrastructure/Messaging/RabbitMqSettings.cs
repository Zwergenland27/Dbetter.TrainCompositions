namespace DBetter.TrainCompositions.Infrastructure.Messaging;

public class RabbitMqSettings
{
    /// <summary>
    /// Section name in appsettings
    /// </summary>
    public const string SectionName = "RabbitMqSettings";

    /// <summary>
    /// Hostname of rabbitmq broker
    /// </summary>
    public string Hostname { get; set; } = null!;

    public void Validate()
    {
        if(Hostname is null)
            throw new ArgumentNullException(nameof(Hostname), $"{SectionName}__{nameof(Hostname)} must be set");
    }
}