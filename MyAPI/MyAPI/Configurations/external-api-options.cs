namespace MyAPI.Configurations;

public sealed class ExternalApiOptions
{
    public const string SectionName = "ExternalApi";

    public string BaseUrl { get; set; } = string.Empty;

    public int TimeoutSeconds { get; set; } = 30;

    public int RetryCount { get; set; } = 2;

    public string ApiKeyHeader { get; set; } = "X-Api-Key";
}
