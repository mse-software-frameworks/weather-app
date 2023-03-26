namespace WeatherApp.Consumer.Config
{
    public class KafkaSettings
    {
        public string Servers { get; set; } = null!;
        public string GroupId { get; set; } = null!;
        public string TopicName { get; set; } = null!;
        public string SchemaRegistryUrl { get; set; } = null!;
    }
}