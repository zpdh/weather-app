using WeatherApp.Domain.Entities;

namespace WeatherApp.Api.Middlewares;

public class ExceptionFilterMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionFilterMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (HttpRequestException httpException)
        {
            await HandleRequestExceptionAsync(context, httpException);
        }
        catch (Exception)
        {
            await HandleUnknownExceptionAsync(context);
        }
    }

    private static async Task HandleRequestExceptionAsync(HttpContext context, HttpRequestException exception)
    {
        context.Response.StatusCode = (int)exception.StatusCode!;
        await context.Response.WriteAsJsonAsync(new ErrorResponse(exception.Message));
    }

    private static async Task HandleUnknownExceptionAsync(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new ErrorResponse("An unknown exception occurred"));
    }
}