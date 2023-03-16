namespace WeatherAggregator;

public class KafkaSettings
{
    public string Topic { get; set; } = null!;
    public string Servers { get; set; } = null!;
    public int Partitions { get; set; }
    public string GroupId { get; set; } = null!;
}