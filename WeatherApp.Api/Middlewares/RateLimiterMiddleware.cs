using StackExchange.Redis;
using WeatherApp.Domain.Entities;

namespace WeatherApp.Api.Middlewares;

public class RateLimiterMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConnectionMultiplexer _redis;

    private const int RateLimit = 10;
    private const int TimeWindowInMinutes = 2;

    public RateLimiterMiddleware(RequestDelegate next, IConnectionMultiplexer redis)
    {
        _next = next;
        _redis = redis;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();

        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            await _next(context);
            return;
        }

        var db = _redis.GetDatabase();
        var cacheKey = $"rateLimit:{ipAddress}";

        var count = await db.StringIncrementAsync(cacheKey);

        switch (count)
        {
            case 1:
                await db.KeyExpireAsync(cacheKey, TimeSpan.FromMinutes(TimeWindowInMinutes));
                break;

            case > RateLimit:
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsJsonAsync(new ErrorResponse("You are sending too many requests. Try again later."));
                return;
        }

        await _next(context);
    }
}