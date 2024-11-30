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
            await HandleRequestException(context, httpException);
        }
        catch (Exception)
        {
            await HandleUnknownException(context);
        }
    }

    private static Task HandleRequestException(HttpContext context, HttpRequestException exception)
    {
        context.Response.StatusCode = (int)exception.StatusCode!;
        return context.Response.WriteAsJsonAsync(new { message = exception.Message });
    }

    private static Task HandleUnknownException(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        return context.Response.WriteAsJsonAsync(new { message = "An unknown exception occurred" });
    }
}