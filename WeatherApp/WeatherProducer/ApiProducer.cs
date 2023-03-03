namespace WeatherProducer;

public class ApiProducer
{
    public async Task Produce(TimeSpan interval, CancellationToken cancellationToken)
    {
        // Async loop
        // https://stackoverflow.com/a/30462232
        while (!cancellationToken.IsCancellationRequested)
        {
            // Produce
            Console.WriteLine("Hey!");
            try { await Task.Delay(interval, cancellationToken); }
            catch (TaskCanceledException) { }
        }
    }
}