using System.Text.Json;
using StackExchange.Redis;
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
            async (HttpClient client, IConnectionMultiplexer redis, string location) => {
                const string timestamp = ApiTimeOptions.Today;

                var cachedData = await GetCachedDataAsync(redis, location, timestamp);

                if (cachedData is not null)
                {
                    return Results.Ok(cachedData);
                }

                var apiData = await GetApiDataAsync(apiKey, location, timestamp, client);
                await SaveDataInRedisAsync(redis, location, timestamp, apiData);

                return Results.Ok(apiData);
            });
    }

    private static async Task<WeatherResult?> GetCachedDataAsync(IConnectionMultiplexer redis, string location, string timestamp)
    {
        var db = redis.GetDatabase();
        var cacheKey = $"weather:{location}:{timestamp}";

        var cachedData = await db.StringGetAsync(cacheKey);

        if (cachedData.IsNullOrEmpty) return null;

        var weatherResult = JsonSerializer.Deserialize<WeatherResult>(cachedData!);

        return weatherResult;

    }

    private static async Task SaveDataInRedisAsync(IConnectionMultiplexer redis, string location, string timestamp, WeatherResult weatherResult)
    {
        var db = redis.GetDatabase();
        var cacheKey = $"weather:{location}:{timestamp}";

        await db.StringSetAsync(cacheKey, JsonSerializer.Serialize(weatherResult), TimeSpan.FromHours(8));
    }

    private static async Task<WeatherResult> GetApiDataAsync(string apiKey, string location, string timestamp, HttpClient client)
    {
        var url = GetApiUrl(apiKey, location, timestamp);

        var result = await client.GetFromJsonAsync<WeatherResult>(url);
        return result!;
    }

    private static string GetApiUrl(string apiKey, string location, string timestamp)
    {
        return
            $"https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/{location}/{timestamp}?key={apiKey}&include=days&elements=tempmax,tempmin,temp";
    }
}