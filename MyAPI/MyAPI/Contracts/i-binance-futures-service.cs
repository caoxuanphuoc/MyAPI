namespace MyAPI.Contracts;

public interface IBinanceFuturesService
{
    Task<(int StatusCode, string Content)> GetBalanceAsync(
        long timestamp,
        string signature,
        int recvWindow,
        string? apiKey,
        CancellationToken cancellationToken = default);
}