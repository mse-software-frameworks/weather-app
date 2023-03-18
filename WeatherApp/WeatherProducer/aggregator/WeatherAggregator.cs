using System.Globalization;
using Streamiz.Kafka.Net;
using Streamiz.Kafka.Net.Crosscutting;
using Streamiz.Kafka.Net.SchemaRegistry.SerDes.Avro;
using Streamiz.Kafka.Net.SerDes;
using Streamiz.Kafka.Net.State;
using Streamiz.Kafka.Net.Table;
using WeatherProducer.AvroSpecific;
using WeatherProducer.config;

namespace WeatherProducer.aggregator;

public class WeatherAggregator
{
    private readonly KafkaConfig _config;

    public WeatherAggregator(KafkaConfig config)
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
        // https://lgouellec.github.io/kafka-streams-dotnet/overview.html
        // Stream (topic) -> Table (memory) -> Stream (topic)
        // Library stores tables in memory or additional databases, not Kafka
        // https://lgouellec.github.io/kafka-streams-dotnet/stores.html
        /*streamBuilder
            .Stream<string, Weather>(_config.WeatherTopic)
            .GroupBy((k, v) => k)
            .Aggregate(Aggregator, (key, value, aggregator) =>
                {
                    AverageTemperature(value, aggregator);
                    AverageWindspeed(value, aggregator);
                    AverageWindchill(value, aggregator);
                    return aggregator;
                }, InMemory.As<string, AverageWeather>(_config.AverageWeatherTable)
                    .WithKeySerdes<StringSerDes>()
                    .WithValueSerdes<SchemaAvroSerDes<AverageWeather>>()
            )
            .ToStream()
            .To<StringSerDes, SchemaAvroSerDes<AverageWeather>>(_config.AverageWeatherTable);*/

        streamBuilder
            .Stream<string, Weather>(_config.WeatherTopic)
            .GroupBy((k, v) => k)
            .Aggregate(Aggregator, (key, value, aggregator) =>
                {
                    AverageTemperature(value, aggregator);
                    AverageWindspeed(value, aggregator);
                    AverageWindchill(value, aggregator);
                    return aggregator;
                }, Materialized<string, AverageWeather, IKeyValueStore<Bytes, byte[]>>
                    .Create(_config.AverageWeatherTable)
                    .WithKeySerdes<StringSerDes>()
                    .WithValueSerdes<SchemaAvroSerDes<AverageWeather>>()
            )
            .ToStream()
            .To<StringSerDes, SchemaAvroSerDes<AverageWeather>>(_config.AverageWeatherTable);

        // streamBuilder.Table("kek",
        //     Materialized<string, AverageWeather, IKeyValueStore<Bytes, byte[]>>
        //         .Create(_config.AverageWeatherTable)
        //         .WithKeySerdes<StringSerDes>()
        //         .WithValueSerdes<SchemaAvroSerDes<AverageWeather>>());

        // Materialized<string, AverageWeather, IKeyValueStore<Bytes, byte[]>>
        // .Create(_config.AverageWeatherTable)
        // .WithKeySerdes<StringSerDes>()
        // .WithValueSerdes<SchemaAvroSerDes<AverageWeather>>()


        // var config2 =
        //     new StreamConfig<StringSerDes, SchemaAvroSerDes<Weather>>
        //     {
        //         ApplicationId = "weather-aggregator2",
        //         BootstrapServers = _config.Servers,
        //         SchemaRegistryUrl = _config.SchemaRegistry
        //     };
        //
        // var streamBuilder2 = new StreamBuilder();
        // streamBuilder2
            
        // streamBuilder
        //     .Stream<string, AverageWeather, StringSerDes, SchemaAvroSerDes<AverageWeather>>(_config.AverageWeatherTable)
        //     .MapValues((k, v) => Format(v.average_temperature))
        //     .To<StringSerDes, StringSerDes>(_config.AverageTemperatureTopic);

        /*var branches = streamBuilder
            .Stream<string, AverageWeather, StringSerDes, SchemaAvroSerDes<AverageWeather>>(_config.AverageWeatherTable)
            .Branch((_, _) => true, (_, _) => true);
        var branch1 = branches[0];
        var branch2 = branches[1];
        branch1
            .MapValues((k, v) => Format(v.average_temperature))
            .To<StringSerDes, StringSerDes>(_config.AverageTemperatureTopic);
        branch2
            .MapValues((k, v) => Format(v.average_windchill))
            .To<StringSerDes, StringSerDes>(_config.AverageWindchillTopic);*/

        // Create stream that can be reused
        // https://stackoverflow.com/a/42418291
        var tableStream = streamBuilder
            .Stream<string, AverageWeather, StringSerDes, SchemaAvroSerDes<AverageWeather>>(
                _config.AverageWeatherTable);
        
        tableStream
            .MapValues((k, v) => Format(v.average_temperature))
            .To<StringSerDes, StringSerDes>(_config.AverageTemperatureTopic);
        tableStream
            .MapValues((k, v) => Format(v.average_windchill))
            .To<StringSerDes, StringSerDes>(_config.AverageWindchillTopic);

        // streamBuilder
        //     .Table<string, AverageWeather>(_config.AverageWeatherTable)
        //     .MapValues((k, v) => v.average_windchill)
        //     .ToStream()
        //     .To<StringSerDes, DoubleSerDes>(_config.AverageWindchillTopic);


        var topology = streamBuilder.Build();
        var stream = new KafkaStream(topology, config);

        // var topology2 = streamBuilder.Build();
        // var stream2 = new KafkaStream(topology2, config2);

        cancellationToken.Register(() => { stream.Dispose(); });

        // var task1 = stream.StartAsync();
        // var task2 = stream2.StartAsync();
        // await Task.WhenAll(task1, task2);
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

    private static void AverageWindspeed(Weather value, AverageWeather aggregator)
    {
        aggregator
            .windspeed_measurements
            .Add(value.current_weather.windspeed);
        aggregator.average_windspeed = aggregator
            .windspeed_measurements
            .Average();
    }

    private static void AverageWindchill(Weather value, AverageWeather aggregator)
    {
        var weather = value.current_weather;
        var windchill = CalculateWindchill(weather.temperature, weather.windspeed);
        aggregator
            .windchill_measurements
            .Add(windchill);
        aggregator.average_windchill = aggregator
            .windchill_measurements
            .Average();
    }

    private static double CalculateWindchill(double temperature, double windspeed)
    {
        // Calculate windchill (temperature in celsius, windspeed in km/h)
        // https://de.wikipedia.org/wiki/Windchill
        return 13.12 + 0.6215 * temperature + (0.3965 * temperature - 11.37) * Math.Pow(windspeed, 0.16);
    }

    private static AverageWeather Aggregator()
    {
        return new AverageWeather
        {
            average_temperature = 0,
            temperature_measurements = new List<double>(),
            windspeed_measurements = new List<double>(),
            windchill_measurements = new List<double>()
        };
    }

    private static string Format(double value)
    {
        return string.Format(new CultureInfo("en-GB"), "{0:0.00}", value);
    }
}