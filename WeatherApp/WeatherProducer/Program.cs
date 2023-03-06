using System.Text;
using Microsoft.Extensions.Configuration;
using WeatherProducer;
using WeatherProducer.config;

Console.OutputEncoding = Encoding.UTF8;
Console.WriteLine("Weather Producer 🌤️");
        
var kafkaConfig = new ConfigurationBuilder()
    .AddJsonFile("config/kafka.json")
    .AddEnvironmentVariables()
    .Build()
    .Get<KafkaConfig>()!;

Console.WriteLine($"Using config: {kafkaConfig}");

// Produce data
var timeSpan = TimeSpan.FromSeconds(1);
var tokenSource = new CancellationTokenSource();
var token = tokenSource.Token;
var producer = new ApiProducer(kafkaConfig);
var task = producer.Produce(timeSpan, token);

// Wait for ctr-c event
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
task.Wait(token);
tokenSource.Dispose();

Console.WriteLine("Shutdown complete");