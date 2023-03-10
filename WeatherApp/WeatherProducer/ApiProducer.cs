using Confluent.Kafka;
using WeatherProducer.config;

namespace WeatherProducer;

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
        using var producer = new ProducerBuilder<Null, string>(producerConfig).Build();
        
        // Async loop
        // https://stackoverflow.com/a/30462232
        var currentPartition = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            // Produce
            var response = await OpenMeteoClient.GetWeatherData();
            if (response != null)
            {
                var partitionId = currentPartition % _config.Partitions;
                currentPartition++;
                response = $"{{\"id\":{partitionId},\"data\":{response}}}";
                Console.WriteLine(response);
                // Write to specific partition
                // https://stackoverflow.com/a/72466351
                var topicPartition = new TopicPartition(_config.Topic, new Partition(partitionId));
                await producer.ProduceAsync(topicPartition, new Message<Null, string>
                {
                    Value = response
                }, cancellationToken);
            }
            try { await Task.Delay(interval, cancellationToken); }
            catch (TaskCanceledException) { }
        }
    }

    private static class OpenMeteoClient
    {
        private static readonly HttpClient Client = new();

        public static async Task<string?> GetWeatherData()
        {
            // Hardcoded Vienna Data
            // https://open-meteo.com/en/docs#api-documentation
            try
            {
                var response = await Client.GetAsync(
                    "https://api.open-meteo.com/v1/forecast?latitude=48.21&longitude=16.37&current_weather=true"
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