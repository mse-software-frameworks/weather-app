namespace WeatherProducer.config;

public class KafkaConfig
{
    public string Servers { get; set; } = null!;
    public string SchemaRegistry { get; set; } = null!;
    public string WeatherTopic { get; set; } = null!;
    public string AverageWeatherTable { get; set; } = null!;
    public string AverageTemperatureTopic { get; set; } = null!;
    public string AverageWindspeedTopic { get; set; } = null!;
    public string AverageWindchillTopic { get; set; } = null!;
    public short Replications { get; set; }
    
    public override string ToString()
    {
        return $"{{Servers={Servers}, SchemaRegistry={SchemaRegistry}, " +
               $"WeatherTopic={WeatherTopic}, AverageWeatherTable={AverageWeatherTable}, " +
               $"AverageWindspeedTopic={AverageWindspeedTopic}, " +
               $"AverageTemperatureTopic={AverageTemperatureTopic}, AverageWindchillTopic={AverageWindchillTopic}, " +
               $"Replications={Replications}}}";
    }

    public string[] Topics()
    {
        return new[] { WeatherTopic, AverageTemperatureTopic, AverageWindspeedTopic, AverageWindchillTopic };
    }
}