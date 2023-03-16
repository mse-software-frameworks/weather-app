using Newtonsoft.Json;

namespace WeatherAggregator;

public class CurrentWeather
{
    public float Temperature { get; set; }

    public float Windspeed { get; set; }

    public float Winddirection { get; set; }

    public int Weathercode { get; set; }

    public DateTime Time { get; set; }

    public CurrentWeather(float temperature, float windspeed, float winddirection, int weathercode, DateTime time)
    {
        Temperature = temperature;
        Windspeed = windspeed;
        Winddirection = winddirection;
        Weathercode = weathercode;
        Time = time;
    }
}