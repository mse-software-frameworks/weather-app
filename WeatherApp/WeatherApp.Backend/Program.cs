using Microsoft.Extensions.Configuration;
using WeatherApp.Backend.Infrastructure;
using WeatherApp.Consumer.Config;

namespace WeatherApp.Consumer;
public class Program
{
    private const string KAFKA_CONFIG_FILEPATH = "config/kafka.json";

    private static void Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var kafkaSettings = config
        .GetSection("Kafka")
        .Get<KafkaSettings>();
        if (kafkaSettings == null)
        {
            Console.Error.WriteLine($"Could not load {nameof(KafkaSettings)} from ${KAFKA_CONFIG_FILEPATH}. Aborting.");
            return;
        }
        
        var connectionString = config.GetConnectionString("MongoDb");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            Console.Error.WriteLine("Database connection string is empty. Aborting.");
            return;
        }

        var cts = new CancellationTokenSource();
        
        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            eventArgs.Cancel = true;
            cts.Cancel();
        };

        var consumer = new Consumer(kafkaSettings, new MongoDbClient(connectionString));
        consumer.Consume(cts.Token);
    }
}