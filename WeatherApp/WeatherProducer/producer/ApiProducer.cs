﻿using System.Text.Json;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Streamiz.Kafka.Net.Errors;
using WeatherApp.Kafka.Schemas.Avro;
using WeatherProducer.config;

namespace WeatherProducer.producer;

public class ApiProducer
{
    private readonly KafkaConfig _config;
    private readonly IList<CitiesConfig.CityConfig> _cities;

    public ApiProducer(KafkaConfig config, CitiesConfig cities)
    {
        _config = config;
        _cities = cities.Cities;
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
            BufferBytes = 100,
            AutoRegisterSchemas = false
        };
        // Weather schema will be automatically registered
        using var producer = 
            new ProducerBuilder<string, Weather>(producerConfig)
                .SetValueSerializer(new AvroSerializer<Weather>(schemaRegistry, avroSerializerConfig))
                .Build();
        
        // Register schemas manually as producer (builder) can only auto register one
        // but multiple ones are needed, e.g. AverageWeather is required later for "WeatherAggregator"
        {
            var subject = SubjectNameStrategy.Topic.ConstructValueSubjectName(_config.WeatherTopic, null);
            var weatherSchema = Weather._SCHEMA.ToString();
            await schemaRegistry.RegisterSchemaAsync(subject, weatherSchema);
            // Use this to modify schema
            // await schemaRegistry.RegisterSchemaAsync(subject, new RegisteredSchema(
            //     subject, 2, 1, weatherSchema, SchemaType.Avro, new List<SchemaReference>()
            // ));
        }
        {
            var subject = SubjectNameStrategy.Topic.ConstructValueSubjectName(_config.AverageWeatherTable, null);
            var averageWeatherSchema = AverageWeather._SCHEMA.ToString();
            await schemaRegistry.RegisterSchemaAsync(subject, averageWeatherSchema);
        }

        // Async loop
        // https://stackoverflow.com/a/30462232
        var currentPartition = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            // Produce
            var partitionId = currentPartition % _cities.Count;
            var city = _cities[partitionId];
            var response = await OpenMeteoClient.GetWeatherData(city.Latitude, city.Longitude);
            if (response != null)
            {
                currentPartition++;
                // Add partition id & city name to response
                response = $"{{\"id\":\"{partitionId}\",\"city\":\"{city.Key}\"," + response[1..];
                Console.WriteLine(response);
                
                // Write to specific partition
                // https://stackoverflow.com/a/72466351
                var topicPartition = new TopicPartition(_config.WeatherTopic, new Partition(partitionId));

                var weatherData = JsonSerializer.Deserialize<Weather>(response);
                if (weatherData != null)
                {
                    await producer.ProduceAsync(topicPartition, new Message<string, Weather>
                    {
                        Key = city.Key,
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