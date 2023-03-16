using System.Globalization;
using Streamiz.Kafka.Net;
using Streamiz.Kafka.Net.SchemaRegistry.SerDes.Avro;
using Streamiz.Kafka.Net.SerDes;
using Streamiz.Kafka.Net.Table;
using WeatherProducer.AvroSpecific;
using WeatherProducer.config;

namespace WeatherProducer;

public class StreamProducer
{
    private readonly KafkaConfig _config;

    public StreamProducer(KafkaConfig config)
    {
        _config = config;
    }
    
    public async Task Produce(CancellationToken cancellationToken)
    {
        var config = 
            new StreamConfig<StringSerDes, SchemaAvroSerDes<Weather>>
            {
                ApplicationId = "weather-aggregator",
                BootstrapServers = _config.Servers,
                SchemaRegistryUrl = _config.SchemaRegistry
            };

        var streamBuilder = new StreamBuilder();

        // Weather temperature aggregation 
        // Stream (topic) -> Table (memory) -> Stream (topic)
        // https://lgouellec.github.io/kafka-streams-dotnet/overview.html
        // https://lgouellec.github.io/kafka-streams-dotnet/stores.html
        streamBuilder
            .Stream<string, Weather>(_config.Topic)
            .GroupBy((k, v) => k)
            .Aggregate(Aggregator, (key, value, aggregator) =>
                {
                    AverageTemperature(value, aggregator);
                    return aggregator;
                }, InMemory.As<string, AverageWeather>("aggregated-weather")
                    .WithKeySerdes<StringSerDes>()
                    .WithValueSerdes<SchemaAvroSerDes<AverageWeather>>()
            )
            .MapValues((k, v) => 
                v.average_temperature.ToString(CultureInfo.InvariantCulture)
            )
            .ToStream()
            .To<StringSerDes, StringSerDes>(_config.AggregateTopic);


        var topology = streamBuilder.Build();
        var stream = new KafkaStream(topology, config);
        
        cancellationToken.Register(() =>
        {
            stream.Dispose();
        });
        
        await stream.StartAsync();
    }

    private static void AverageTemperature(Weather value, AverageWeather aggregator)
    {
        aggregator
            .temperature_measurements
            .Add(value.current_weather.temperature);
        aggregator.average_temperature = aggregator
            .temperature_measurements
            .Average();;
    }

    private static AverageWeather Aggregator()
    {
        return new AverageWeather
        {
            average_temperature = 0,
            temperature_measurements = new List<double>()
        };
    }
}