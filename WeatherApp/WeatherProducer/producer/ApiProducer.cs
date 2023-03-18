using System.Text.Json;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using WeatherProducer.AvroSpecific;
using WeatherProducer.config;

namespace WeatherProducer.producer;

public class ApiProducer
{
    private readonly KafkaConfig _config;

    public ApiProducer(KafkaConfig config)
    {
        _config = config;
    }
    
    public async Task Produce(TimeSpan interval, CancellationToken cancellationToken)
    {
        // Setup kafka producer
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = _config.Servers
        };

        var schemaRegistryConfig = new SchemaRegistryConfig
        {
            Url = _config.SchemaRegistry
        };
        using var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);
        
        var avroSerializerConfig = new AvroSerializerConfig
        {
            // optional Avro serializer properties:
            BufferBytes = 100
        };
        using var producer = 
            new ProducerBuilder<string, Weather>(producerConfig)
                .SetValueSerializer(new AvroSerializer<Weather>(schemaRegistry, avroSerializerConfig))
                .Build();
        
        // Async loop
        // https://stackoverflow.com/a/30462232
        var currentPartition = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            // Produce
            // var partitionId = currentPartition % _config.Partitions;
            var partitionId = 0;
            var response = await OpenMeteoClient.GetWeatherData(partitionId);
            if (response != null)
            {
                currentPartition++;
                // Add partition id to response
                response = $"{{\"id\":\"{partitionId}\"," + response[1..];
                Console.WriteLine(response);
                
                // Write to specific partition
                // https://stackoverflow.com/a/72466351
                var topicPartition = new TopicPartition(_config.WeatherTopic, new Partition(partitionId));

                var weatherData = JsonSerializer.Deserialize<Weather>(response);
                if (weatherData != null)
                {
                    await producer.ProduceAsync(topicPartition, new Message<string, Weather>
                    {
                        Key = partitionId.ToString(),
                        Value = weatherData
                    }, cancellationToken);
                }
            }
            try { await Task.Delay(interval, cancellationToken); }
            catch (TaskCanceledException) { }
        }
    }
    
    

    private static class OpenMeteoClient
    {
        private static readonly HttpClient Client = new();

        private static readonly Dictionary<string, Tuple<string, string>> Cities = new()
        {
            { "Vienna", new Tuple<string, string>("48.21", "16.37") },
            { "London", new Tuple<string, string>("51.51", "-0.13") },
            { "Berlin", new Tuple<string, string>("52.52", "13.41") },
        };

        public static async Task<string?> GetWeatherData(int currentPartition)
        {
            var coordinates = Cities.Values.ElementAt(currentPartition);
            return await GetWeatherData(coordinates.Item1, coordinates.Item2);
        }

        public static async Task<string?> GetWeatherData(string latitude, string longitude)
        {
            // Hardcoded Vienna Data
            // https://open-meteo.com/en/docs#api-documentation
            try
            {
                var response = await Client.GetAsync(
                    $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&current_weather=true"
                );
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                await Console.Error.WriteLineAsync(ex.Message);
                return null;
            }
        }
    }
}