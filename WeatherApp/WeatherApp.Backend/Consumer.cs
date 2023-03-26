using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using WeatherApp.Backend.Infrastructure;
using WeatherApp.Consumer.Config;
using WeatherApp.Kafka.Schemas.Avro;

namespace WeatherApp.Consumer;

public class Consumer
{
    private readonly KafkaSettings _kafkaSettings;
    private readonly IDbClient _dbClient;

    public Consumer(KafkaSettings kafkaSettings, IDbClient dbClient)
    {
        _kafkaSettings = kafkaSettings;
        _dbClient = dbClient;
    }

    public void Consume(CancellationToken cancellationToken)
    {
        var schemaRegistryConfig = new SchemaRegistryConfig()
        {
            Url = _kafkaSettings.SchemaRegistryUrl
        };

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _kafkaSettings.Servers,
            GroupId = _kafkaSettings.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);
        using var consumer = new ConsumerBuilder<string, AggregatedWeather>(consumerConfig)
            .SetValueDeserializer(new AvroDeserializer<AggregatedWeather>(schemaRegistry).AsSyncOverAsync())
            .Build();

        consumer.Subscribe(_kafkaSettings.TopicName);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(cancellationToken);
                Console.WriteLine(result.Message.Value.AverageTemperature);

                _dbClient.InsertData(result.Message.Key, result.Message.Value);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Aborting...");
                return;
            }
        }
    }
}
