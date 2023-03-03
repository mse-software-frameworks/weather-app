using Confluent.Kafka;
using WeatherProducer.config;

namespace WeatherProducer;

public class ApiProducer
{
    private readonly KafkaConfig config;

    public ApiProducer(KafkaConfig config)
    {
        this.config = config;
    }
    
    
    public async Task Produce(TimeSpan interval, CancellationToken cancellationToken)
    {
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = config.Servers
        };
        using var producer = new ProducerBuilder<Null, string>(producerConfig).Build();
        
        // Async loop
        // https://stackoverflow.com/a/30462232
        while (!cancellationToken.IsCancellationRequested)
        {
            // Produce
            Console.WriteLine("Crunching data...");
            await producer.ProduceAsync(config.Topic, new Message<Null, string>
            {
                Value = "Weather Data"
            }, cancellationToken);
            try { await Task.Delay(interval, cancellationToken); }
            catch (TaskCanceledException) { }
        }
    }
}