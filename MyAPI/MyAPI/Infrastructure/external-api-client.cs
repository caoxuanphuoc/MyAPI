using MyAPI.Contracts;

namespace MyAPI.Infrastructure;

public sealed class ExternalApiClient(HttpClient httpClient) : IExternalApiClient
{
    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        return httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
    }
}
