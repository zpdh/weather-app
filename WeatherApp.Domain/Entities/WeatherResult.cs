namespace WeatherApp.Domain.Entities;

public class WeatherResult
{
    public string ResolvedAddress { get; set; } = string.Empty;
    public List<Day>? Days { get; set; }


}