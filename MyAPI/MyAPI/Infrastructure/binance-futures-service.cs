using System.Net.Http.Headers;
using MyAPI.Configurations;
using MyAPI.Contracts;
using Microsoft.Extensions.Options;

namespace MyAPI.Infrastructure;

public sealed class BinanceFuturesService(
    IExternalApiClient externalApiClient,
    IOptions<BinanceFuturesOptions> options) : IBinanceFuturesService
{
    private readonly BinanceFuturesOptions _options = options.Value;

    public async Task<(int StatusCode, string Content)> GetBalanceAsync(
        long timestamp,
        string signature,
        int recvWindow,
        string? apiKey,
        CancellationToken cancellationToken = default)
    {
        var requestUri = BuildBalanceUri(timestamp, signature, recvWindow);
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

        var effectiveApiKey = string.IsNullOrWhiteSpace(apiKey) ? _options.ApiKey : apiKey;
        if (!string.IsNullOrWhiteSpace(effectiveApiKey))
        {
            request.Headers.TryAddWithoutValidation(_options.ApiKeyHeader, effectiveApiKey);
        }

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var response = await externalApiClient.SendAsync(request, cancellationToken);
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);
        return ((int)response.StatusCode, payload);
    }

    private Uri BuildBalanceUri(long timestamp, string signature, int recvWindow)
    {
        var normalizedBase = _options.BaseUrl.TrimEnd('/');
        var escapedSignature = Uri.EscapeDataString(signature);
        var uri =
            $"{normalizedBase}/fapi/v3/balance?timestamp={timestamp}&signature={escapedSignature}&recvWindow={recvWindow}";

        return new Uri(uri, UriKind.Absolute);
    }
}