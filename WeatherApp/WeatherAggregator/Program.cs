using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Streamiz.Kafka.Net;
using Streamiz.Kafka.Net.SerDes;
using Streamiz.Kafka.Net.Table;
using WeatherAggregator;
using WeatherConsumer;

var kafkaSettings = new ConfigurationBuilder()
    .AddJsonFile("kafka.settings.json")
    .Build()
    .Get<KafkaSettings>();

var config = new StreamConfig<StringSerDes, StringSerDes>
{
    ApplicationId = kafkaSettings.GroupId,
    BootstrapServers = kafkaSettings.Servers
};

var streamBuilder = new StreamBuilder();

streamBuilder.Stream<string, string>(kafkaSettings.Topic)
    .MapValues((string key, string value) =>
    {
        return JsonConvert.DeserializeObject<WeatherData>(value);
    })
    .GroupByKey()
    .Aggregate(
        () => new AverageTemperature(),
        (key, value, aggregate) =>
        {
            if (value?.CurrentWeather?.Temperature != null)
                aggregate.Temperatures.Add(value.CurrentWeather.Temperature);

            return aggregate;
        },
        InMemory.As<string, AverageTemperature>("test-table")
            .WithKeySerdes<StringSerDes>()
            .WithValueSerdes<JsonSerDes<AverageTemperature>>()
    )
    .ToStream()
    .MapValues(value => value.Temperatures.Average())
    .To<StringSerDes, FloatSerDes>("aggregate");


var topology = streamBuilder.Build();

var stream = new KafkaStream(topology, config);

await stream.StartAsync();

Console.ReadLine();