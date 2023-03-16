using Newtonsoft.Json;

namespace WeatherAggregator;

public class WeatherData
{
    public float Latitude { get; set; }

    public float Longitude { get; set; }

    public double GenerationtimeMs { get; set; }

    public int UtcOffsetSeconds { get; set; }

    public string Timezone { get; set; }

    public string TimezoneAbbreviation { get; set; }

    public float Elevation { get; set; }

    [JsonProperty("current_weather")]
    public CurrentWeather CurrentWeather { get; set; }

    public WeatherData(float latitude, float longitude, double generationtimeMs, int utcOffsetSeconds, string timezone, string timezoneAbbreviation, float elevation, CurrentWeather currentWeather)
    {
        Latitude = latitude;
        Longitude = longitude;
        GenerationtimeMs = generationtimeMs;
        UtcOffsetSeconds = utcOffsetSeconds;
        Timezone = timezone;
        TimezoneAbbreviation = timezoneAbbreviation;
        Elevation = elevation;
        CurrentWeather = currentWeather;
    }
}