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

        public override string ToString()
        {
            return $"{{{nameof(Id)}: {Id}, " +
                $"{nameof(City)}: {City}, " +
                $"{nameof(Date)}: {Date}, " +
                $"{nameof(AverageTemperature)}: {AverageTemperature}, " +
                $"{nameof(AverageWindspeed)}: {AverageWindspeed}, " +
                $"{nameof(AverageWindchill)}: {AverageWindchill}}}";
        }
    }
}
