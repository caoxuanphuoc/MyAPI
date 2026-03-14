namespace MyAPI.Contracts;

public interface IExternalApiClient
{
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
}
