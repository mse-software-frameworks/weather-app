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
        while (!cancellationToken.IsCancellationRequested)
        {
            // Produce
            var response = await OpenMeteoClient.GetWeatherData();
            if (response != null)
            {
                Console.WriteLine(response);
                await producer.ProduceAsync(_config.Topic, new Message<Null, string>
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