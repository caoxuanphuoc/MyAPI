namespace MyAPI.Models.ExternalApi;

public sealed class ExternalApiError
{
    public int StatusCode { get; init; }

    public string Message { get; init; } = string.Empty;
}
