using System.Net;
using ActionCache.AzureCosmos;
using ActionCache.Common;
using ActionCache.Common.Caching;
using ActionCache.Common.Serialization;
using ActionCache.Utilities;
using Microsoft.Azure.Cosmos;

namespace ActionCache.Memory;

/// <summary>
/// Represents an Azure Cosmos Db action cache implementation.
/// </summary>
public class AzureCosmosActionCache : ActionCacheBase
{
    /// <summary>
    /// an Azure Cosmos Db cache implementation.
    /// </summary>
    protected readonly Container Cache;

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
    }

    /// <summary>
    /// Asynchronously gets a value from the cache.
    /// </summary>
    /// <param name="key">The key of the cache entry.</param>
    /// <returns>The cached value or null if not found.</returns> 
    public override async Task<TValue> GetAsync<TValue>(string key)
    {
        var response = await Cache.ReadItemAsync<AzureCosmosEntry>(
            Namespace.Create(key), 
            new PartitionKey(Namespace)
        );

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var value = CacheJsonSerializer.Deserialize<TValue>(response.Resource.Value);
            return value;
        }
        else
        {
            return default;
        }
    }

    /// <summary>
    /// Asynchronously sets a value in the cache.
    /// </summary>
    /// <param name="key">The cache key to set the value for.</param>
    /// <param name="value">The value to set in the cache.</param>
    public override async Task SetAsync<TValue>(string key, TValue value)
    {
        var response = await Cache.UpsertItemAsync(new AzureCosmosEntry
        {
            Id = Namespace.Create(key),
            Namespace = Namespace,
            Value = CacheJsonSerializer.Serialize(value)
        }, new PartitionKey(Namespace));
        
        if (response.StatusCode is not HttpStatusCode.OK)
        {
            // TODO: Log or throw error
        }
    }

    /// <summary>
    /// Asynchronously removes a value from the cache.
    /// </summary>
    /// <param name="key">The key of the cache entry to remove.</param>
    public override async Task RemoveAsync(string key)
    {
        var response = await Cache.DeleteItemAsync<AzureCosmosEntry>(
            Namespace.Create(key), 
            new PartitionKey(Namespace)
        );

        if (response.StatusCode is not HttpStatusCode.NoContent)
        {
            // TODO: Retry attempts to make cache consistent
        }
    }

    /// <summary>
    /// Asynchronously removes all values from the cache.
    /// </summary>
    public override Task RemoveAsync() =>
        Cache.DeleteAllItemsByPartitionKeyStreamAsync(
            new PartitionKey(Namespace));

    /// <summary>
    /// Retrieves all keys associated with this cache.
    /// </summary>
    /// <returns>An enumerable of strings representing current cache entry keys.</returns>
    public override Task<IEnumerable<string>> GetKeysAsync()
    {
        var requestOptions = new QueryRequestOptions
        {
            PartitionKey = new PartitionKey(Namespace)
        };

        var result = Cache.GetItemLinqQueryable<AzureCosmosEntry>(requestOptions: requestOptions)
            .Select(element => element.Id)
            .AsEnumerable();

        return Task.FromResult(result);
    }
}