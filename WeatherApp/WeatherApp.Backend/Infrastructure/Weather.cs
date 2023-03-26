using MongoDB.Bson;

namespace WeatherApp.Backend.Infrastructure
{
    public class Weather
    {
        public ObjectId Id { get; set; }
        public string City { get; set; } = null!;
        public DateTime Date { get; set; }
        public string AverageTemperature { get; set; } = null!;
        public string AverageWindspeed { get; set; } = null!;
        public string AverageWindchill { get; set; } = null!;
    }
}
