namespace WeatherProducer.config;

public class CitiesConfig
{
    public class CityConfig
    {
        public int Partition { get; set; }
        public string Key { get; set; } = null!;
        public string Latitude { get; set; } = null!;
        public string Longitude { get; set; } = null!;
        
        public override string ToString()
        {
            return $"{{Partition={Partition}, Key={Key}, Latitude={Latitude}, Longitude={Longitude}}}";
        }
    }

    public List<CityConfig> Cities { get; set; } = null!;

    public override string ToString()
    {
        return $"{{{string.Join(", ", Cities)}}}";
    }
}