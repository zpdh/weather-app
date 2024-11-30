using WeatherApp.Domain.Entities;

namespace WeatherApp.Api.Endpoints;

public static class WeatherEndpoints
{
    public static void MapEndpoints(WebApplication app, string apiKey)
    {
        MapTodayWeatherEndpoint(app, apiKey);
    }

    private static void MapTodayWeatherEndpoint(WebApplication app, string apiKey)
    {
        app.MapGet("weather-today",
            async (HttpClient client, string location) => {
                var url = GetApiUrl(apiKey, location, ApiTimeOptions.Today);

                var result = await client.GetFromJsonAsync<WeatherResult>(url);

                return Results.Ok(result);
            });
    }

    private static string GetApiUrl(string apiKey, string location, string timestamp)
    {
        return
            $"https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/{location}/{timestamp}?key={apiKey}&include=days&elements=tempmax,tempmin,temp";
    }
}