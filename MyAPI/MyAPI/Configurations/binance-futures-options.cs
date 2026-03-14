namespace MyAPI.Configurations;

public sealed class BinanceFuturesOptions
{
    public const string SectionName = "BinanceFutures";

    public string BaseUrl { get; set; } = "https://fapi.binance.com/";

    public string ApiKeyHeader { get; set; } = "X-MBX-APIKEY";

    public string ApiKey { get; set; } = string.Empty;
}