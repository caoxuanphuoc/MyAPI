using MyAPI.Models.ExternalApi;

namespace MyAPI.Contracts;

public interface IExternalDataService
{
    Task<IReadOnlyList<ExternalApiItem>> GetItemsAsync(CancellationToken cancellationToken = default);

    Task<ExternalApiItem?> GetItemByIdAsync(string id, CancellationToken cancellationToken = default);

    Task<ExternalApiItem> CreateItemAsync(ExternalApiItem item, CancellationToken cancellationToken = default);

    Task<ExternalApiItem?> UpdateItemAsync(string id, ExternalApiItem item, CancellationToken cancellationToken = default);

    Task<bool> DeleteItemAsync(string id, CancellationToken cancellationToken = default);
}
