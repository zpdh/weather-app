namespace WeatherApp.Domain.Entities;

public class ErrorResponse
{
    public string Error { get; set; } = string.Empty;

    public ErrorResponse(string error)
    {
        Error = error;
    }

    public ErrorResponse()
    {

    }
}