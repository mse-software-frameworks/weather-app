namespace WeatherProducer.config;

public class KafkaConfig
{
    public string Servers { get; set; } = null!;
    public string SchemaRegistry { get; set; } = null!;
    public string WeatherTopic { get; set; } = null!;
    public string AverageWeatherTable { get; set; } = null!;
    public string AggregatedWeatherTopic { get; set; } = null!;
    public string AverageTemperatureTopic { get; set; } = null!;
    public string AverageWindspeedTopic { get; set; } = null!;
    public string AverageWindchillTopic { get; set; } = null!;
    public short Replications { get; set; }
    public string StreamApplicationId { get; set; } = null!;
    
    public override string ToString()
    {
        return $"{{Servers={Servers}, SchemaRegistry={SchemaRegistry}, " +
               $"WeatherTopic={WeatherTopic}, AverageWeatherTable={AverageWeatherTable}, " +
               $"AverageWindspeedTopic={AverageWindspeedTopic}, " +
               $"AverageTemperatureTopic={AverageTemperatureTopic}, AverageWindchillTopic={AverageWindchillTopic}, " +
               $"Replications={Replications}, StreamApplicationId={StreamApplicationId}}}";
    }

    public string[] Topics()
    {
        return new[] { WeatherTopic, AggregatedWeatherTopic, AverageTemperatureTopic, AverageWindspeedTopic, AverageWindchillTopic };
    }

    public string[] Tables()
    {
        return new[] { AverageWeatherTable };
    }
}