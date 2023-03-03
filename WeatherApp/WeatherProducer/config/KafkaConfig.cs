namespace WeatherProducer.config;

public class KafkaConfig
{
    public string Topic { get; set; } = default!;
    public string Servers { get; set; } = default!;

    public override string ToString()
    {
        return $"{{Topic={Topic}, Servers={Servers}}}";
    }
}