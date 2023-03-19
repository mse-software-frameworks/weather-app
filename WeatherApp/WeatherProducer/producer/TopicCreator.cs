using Confluent.Kafka;
using Confluent.Kafka.Admin;
using WeatherProducer.config;

namespace WeatherProducer.producer;

public class TopicCreator
{
    private readonly KafkaConfig _config;
    private readonly CitiesConfig _cities;

    public TopicCreator(KafkaConfig config, CitiesConfig cities)
    {
        _config = config;
        _cities = cities;
    }

    public async Task Create()
    {
        // Create kafka topics
        // https://stackoverflow.com/a/55849553
        var adminConfig = new AdminClientConfig
        {
            BootstrapServers = _config.Servers
        };
        using var adminClient = new AdminClientBuilder(adminConfig).Build();

        var partitions = _cities.cities.Count;
        var replications = _config.Replications;
        var topicSpecifications = _config
            .Topics()
            .Select(name => new TopicSpecification
            {
                Name = name,
                NumPartitions = partitions,
                ReplicationFactor = replications
            }).ToArray();
        var applicationId = _config.StreamApplicationId;
        var deleteTopics =
            _config.Topics()
            .Concat(_config.Tables()
                .SelectMany(table =>
                {
                    // "Streamiz.Kafka.Net" uses this format for table topics
                    // Tables are automatically generated in "WeatherAggregator.cs"
                    return new[]
                    {
                        $"{applicationId}-{table}-changelog",
                        $"{applicationId}-{table}-repartition"
                    };
                }));

        try
        {
            await adminClient.DeleteTopicsAsync(deleteTopics);
        }
        catch (DeleteTopicsException ex)
        {
        }

        // Wait a bit as topic deletion can take a while...
        // If executed immediately could lead to exception "topic xyz marked for deletion"...
        await Task.Delay(1250);
        await adminClient.CreateTopicsAsync(topicSpecifications);
    }
}