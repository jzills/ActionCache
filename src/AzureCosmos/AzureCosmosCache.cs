using System.Net;
using Microsoft.Azure.Cosmos;
using ActionCache.Common;
using ActionCache.Common.Caching;
using ActionCache.Common.Serialization;
using ActionCache.Utilities;
using ActionCache.AzureCosmos.Extensions;

namespace ActionCache.AzureCosmos;

/// <summary>
/// Represents an Azure Cosmos Db action cache implementation.
/// </summary>
public class AzureCosmosActionCache : ActionCacheBase
{
    /// <summary>
    /// An Azure Cosmos Db cache implementation.
    /// </summary>
    protected readonly Container Cache;

    /// <summary>
    /// The namespaced partition key.
    /// </summary> 
    protected readonly PartitionKey PartitionKey;

    /// <summary>
    /// Initializes a new instance of the MemoryActionCache class.
    /// </summary>
    /// <param name="namespace">The namespace for cache isolation.</param>
    /// <param name="cache">The Azure Cosmos Db container instance.</param>
    /// <param name="entryOptions">The global entry options used for creation when expiration times are not supplied.</param> 
    /// <param name="refreshProvider">The refresh provider to handle cache refreshes.</param>
    public AzureCosmosActionCache(
        Namespace @namespace,
        Container cache,
        ActionCacheEntryOptions entryOptions,
        IActionCacheRefreshProvider refreshProvider
    ) : base(@namespace, entryOptions, refreshProvider)
    {
        Cache = cache;
        PartitionKey = new PartitionKey(Namespace);
    }

    /// <summary>
    /// Asynchronously gets a value from the cache.
    /// </summary>
    /// <param name="key">The key of the cache entry.</param>
    /// <returns>The cached value or null if not found.</returns> 
    public override async Task<TValue> GetAsync<TValue>(string key)
    {
        try
        {
            var response = await Cache.ReadItemAsync<AzureCosmosEntry>(
                Namespace.Create(key), 
                PartitionKey
            );

            return CacheJsonSerializer.Deserialize<TValue>(response.Resource.Value);
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            // Silence errors for entries not found on a particular key.
            return default;
        }
    }

    /// <summary>
    /// Asynchronously sets a value in the cache.
    /// </summary>
    /// <param name="key">The cache key to set the value for.</param>
    /// <param name="value">The value to set in the cache.</param>
    public override Task SetAsync<TValue>(string key, TValue value) =>
        Cache.UpsertItemAsync(new AzureCosmosEntry
        {
            Id = Namespace.Create(key),
            Namespace = Namespace,
            Value = CacheJsonSerializer.Serialize(value)
        }, PartitionKey);

    /// <summary>
    /// Asynchronously removes a value from the cache.
    /// </summary>
    /// <param name="key">The key of the cache entry to remove.</param>
    public override Task RemoveAsync(string key) => 
        Cache.DeleteItemAsync<AzureCosmosEntry>(
            Namespace.Create(key), 
            PartitionKey
        );

    /// <summary>
    /// Asynchronously removes all values from the cache.
    /// </summary>
    public override async Task RemoveAsync()
    {
        var response = await Cache.DeleteAllItemsByPartitionKeyStreamAsync(PartitionKey);
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var itemIds = await Cache.GetItemIdsAsync(Namespace);
            if (itemIds.Any())
            {
                var batch = Cache.CreateTransactionalBatch(PartitionKey);
                foreach (var itemId in itemIds)
                {
                    batch.DeleteItem(itemId);
                }

                var batchResponse = await batch.ExecuteAsync();
                if (!batchResponse.IsSuccessStatusCode)
                {
                    throw new Exception(batchResponse.ErrorMessage);
                }
            }
        }
    }

    /// <summary>
    /// Retrieves all keys associated with this cache.
    /// </summary>
    /// <returns>An enumerable of strings representing current cache entry keys.</returns>
    public override Task<IEnumerable<string>> GetKeysAsync() => Cache.GetItemIdsAsync(Namespace);
}