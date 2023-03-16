namespace WeatherProducer.config;

public class KafkaConfig
{
    public string Topic { get; set; } = null!;
    public string AggregateTopic { get; set; } = null!;
    public string Servers { get; set; } = null!;
    public int Partitions { get; set; }

    public string SchemaRegistry { get; set; } = null!;

    public override string ToString()
    {
        return $"{{Topic={Topic}, Servers={Servers}, Partitions={Partitions}, SchemaRegistry={SchemaRegistry}, AggregateTopic={AggregateTopic}}}";
    }
}