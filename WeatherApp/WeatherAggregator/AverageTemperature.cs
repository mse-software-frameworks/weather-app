using Newtonsoft.Json;

namespace WeatherConsumer;

public class AverageTemperature
{
    [JsonProperty]
    public List<float> Temperatures { get; set; } = new();

    //public float CalculateAverageTemperature()
    //{
    //    return Temperatures.Average();
    //}

    //public void Add(float temperature)
    //{
    //    Temperatures.Add(temperature);
    //}
}