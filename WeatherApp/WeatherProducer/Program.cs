using System.Text;
using Microsoft.Extensions.Configuration;
using WeatherProducer.aggregator;
using WeatherProducer.config;
using WeatherProducer.producer;

Console.OutputEncoding = Encoding.UTF8;
Console.WriteLine("Weather Producer 🌤️");
Console.WriteLine("Waiting for other containers...");
// Helps reducing the amount of restarts needed because backend
// crashes until kafka cluster is online
Thread.Sleep(20000);
Console.WriteLine("Initializing backend");

var kafkaConfig = new ConfigurationBuilder()
    .AddJsonFile("config/kafka.json")
    .AddEnvironmentVariables()
    .Build()
    .Get<KafkaConfig>()!;

Console.WriteLine($"Using config: {kafkaConfig}");

var citiesConfig = new ConfigurationBuilder()
    .AddJsonFile("config/cities.json")
    .AddEnvironmentVariables()
    .Build()
    .Get<CitiesConfig>();

Console.WriteLine($"Set cities to track: {citiesConfig}");

// Delete & create topics
var topicCreator = new TopicCreator(kafkaConfig, citiesConfig);
try
{
    await topicCreator.Create();
}
catch (Exception ex)
{
    // DeleteTopicsException, CreateTopicsException & and so on
    Console.Error.WriteLine($"An error occured managing topics: {ex.Message})");
    return;
}

// Produce data
var timeSpan = TimeSpan.FromSeconds(1);
var tokenSource = new CancellationTokenSource();
var token = tokenSource.Token;
var tasks = new List<Task>();

// https://stackoverflow.com/a/41900329

// Aggregates raw data
var weatherAggregator = new WeatherAggregator(kafkaConfig);
tasks.Add(weatherAggregator.Produce(token)
    .ContinueWith(task => tokenSource.Cancel(), token, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Default)
);

// Produces raw data from api
var apiProducer = new ApiProducer(kafkaConfig, citiesConfig);
tasks.Add(
    apiProducer.Produce(timeSpan, token)
        .ContinueWith(task =>
        {
            Console.WriteLine("Baum");
            tokenSource.Cancel();
        }, token, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Default)
);

tasks.Add(Task.Run(() =>
{
    // Wait for ctrl-c event
    // https://stackoverflow.com/a/13899429
    var exitEvent = new ManualResetEvent(false);
    token.Register(() => exitEvent.Set());
    Console.CancelKeyPress += (sender, eventArgs) =>
    {
        eventArgs.Cancel = true;
        exitEvent.Set();
    };
    exitEvent.WaitOne();
    Console.WriteLine("Initiating shutdown...");
    tokenSource.Cancel();
}));


Task.WaitAll(tasks.ToArray());

tokenSource.Dispose();

Console.WriteLine("Shutdown complete");

// It seems that Environment.Exit(1) does not restart container
throw new System.AggregateException("Restart!");