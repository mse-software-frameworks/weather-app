using System.Text;
using Microsoft.Extensions.Configuration;
using WeatherProducer;
using WeatherProducer.config;
using WeatherProducer.producer;

Console.OutputEncoding = Encoding.UTF8;
Console.WriteLine("Weather Producer 🌤️");
        
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

var kek = "";


/*// Produce data
var timeSpan = TimeSpan.FromSeconds(1);
var tokenSource = new CancellationTokenSource();
var token = tokenSource.Token;
var tasks = new List<Task>();
var apiProducer = new ApiProducer(kafkaConfig);
tasks.Add(apiProducer.Produce(timeSpan, token));
var weatherAggregator = new WeatherAggregator(kafkaConfig);
tasks.Add(weatherAggregator.Produce(token));

// Wait for ctrl-c event
// https://stackoverflow.com/a/13899429
var exitEvent = new ManualResetEvent(false);
Console.CancelKeyPress += (sender, eventArgs) =>
{
    eventArgs.Cancel = true;
    exitEvent.Set();
};
exitEvent.WaitOne();
        
// Cancel producer via token
Console.WriteLine("Initiating shutdown...");
tokenSource.Cancel();
try
{
    Task.WaitAll(tasks.ToArray(), token);
}
catch (Exception ex) {}

tokenSource.Dispose();

Console.WriteLine("Shutdown complete");*/