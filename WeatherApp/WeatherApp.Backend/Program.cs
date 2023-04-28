using Microsoft.Extensions.Configuration;
using WeatherApp.Backend.Infrastructure;
using WeatherApp.Consumer.Config;

namespace WeatherApp.Consumer;
public class Program
{
    private const string KAFKA_CONFIG_FILEPATH = "config/kafka.json";

    private static void Main(string[] args)
    {
        Console.WriteLine("Waiting for other containers...");
        // Helps reducing the amount of restarts needed because backend
        // crashes until kafka cluster is online
        Thread.Sleep(25000);
        Console.WriteLine("Initializing backend");
        
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
        var consumer = new Consumer(kafkaSettings, new MongoDbClient(connectionString));

        var tasks = new List<Task>();
        tasks.Add(
            consumer.Consume(cts.Token)
                .ContinueWith(task => cts.Cancel(), cts.Token, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Default));
        
        tasks.Add(Task.Run(() =>
        {
            // Wait for ctrl-c event
            // https://stackoverflow.com/a/13899429
            var exitEvent = new ManualResetEvent(false);
            cts.Token.Register(() => exitEvent.Set());
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                exitEvent.Set();
            };
            exitEvent.WaitOne();
            Console.WriteLine("Initiating shutdown...");
            cts.Cancel();
        }));


        Task.WaitAll(tasks.ToArray());

        cts.Dispose();

        Console.WriteLine("Shutdown complete");

        // It seems that Environment.Exit(1) does not restart container
        throw new System.AggregateException("Restart!");
    }
}