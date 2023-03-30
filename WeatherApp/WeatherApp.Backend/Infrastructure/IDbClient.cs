using WeatherApp.Kafka.Schemas.Avro;

namespace WeatherApp.Backend.Infrastructure
{
    public interface IDbClient
    {
        void InsertData(string city, AggregatedWeather aggregatedWeather);
    }
}