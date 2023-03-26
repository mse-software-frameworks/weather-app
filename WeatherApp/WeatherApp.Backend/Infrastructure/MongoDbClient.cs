using MongoDB.Driver;
using WeatherApp.Kafka.Schemas.Avro;

namespace WeatherApp.Backend.Infrastructure
{
    public class MongoDbClient : IDbClient
    {
        private MongoClient _client;

        public MongoDbClient(string connectionString)
        {
            _client = new MongoClient(connectionString);
        }

        public void InsertData(string city, AggregatedWeather aggregatedWeather)
        {
            var weather = new Weather
            {
                City = city,
                Date = DateTime.Now,
                AverageTemperature = aggregatedWeather.AverageTemperature,
                AverageWindspeed = aggregatedWeather.AverageWindspeed,
                AverageWindchill = aggregatedWeather.AverageWindchill,
            };

            _client.GetDatabase("weatherAppDb")
                .GetCollection<Weather>(nameof(Weather))
                .InsertOne(weather);
        }
    }
}
