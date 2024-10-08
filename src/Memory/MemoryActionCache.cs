using ActionCache.Common.Caching;
using ActionCache.Memory.Extensions.Internal;
using ActionCache.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace ActionCache.Memory;

/// <summary>
/// Represents a memory action cache implementation.
/// </summary>
public class MemoryActionCache : Common.Caching.ActionCache//IActionCache
{
    protected readonly IMemoryCache Cache;
    protected readonly CancellationTokenSource CancellationTokenSource;

    /// <summary>
    /// Initializes a new instance of the MemoryActionCache class.
    /// </summary>
    /// <param name="namespace">The namespace for cache isolation.</param>
    /// <param name="cache">The memory cache instance.</param>
    /// <param name="cancellationTokenSource">The source for cancellation tokens used to expire cache entries.</param>
    /// <param name="refreshProvider">The refresh provider to handle cache refreshes.</param>
    public MemoryActionCache(
        Namespace @namespace,
        IMemoryCache cache,
        CancellationTokenSource cancellationTokenSource,
        ActionCacheRefreshProvider refreshProvider
    ) : base(@namespace, refreshProvider)
    {
        Cache = cache;
        CancellationTokenSource = cancellationTokenSource;
    }

    /// <summary>
    /// Gets the entry options for memory cache.
    /// </summary>
    /// <value>The cache entry options applied to new entries.</value>
    public MemoryCacheEntryOptions EntryOptions =>
        new MemoryCacheEntryOptions { Size = 1 }
            .AddExpirationToken(
                new CancellationChangeToken(CancellationTokenSource.Token));

    /// <summary>
    /// Asynchronously gets a value from the cache.
    /// </summary>
    /// <param name="key">The key of the cache entry.</param>
    /// <returns>The cached value or null if not found.</returns> 
    public override Task<TValue> GetAsync<TValue>(string key) =>
        Task.FromResult(Cache.Get<TValue?>(Namespace.Create(key)));

    /// <summary>
    /// Asynchronously sets a value in the cache.
    /// </summary>
    /// <param name="key">The cache key to set the value for.</param>
    /// <param name="value">The value to set in the cache.</param>
    public override Task SetAsync<TValue>(string key, TValue value)
    {
        Cache.Set(Namespace.Create(key), value, EntryOptions);
        Cache.SetKey(Namespace, key, EntryOptions);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Asynchronously removes a value from the cache.
    /// </summary>
    /// <param name="key">The key of the cache entry to remove.</param>
    public override Task RemoveAsync(string key)
    {
        Cache.Remove(Namespace.Create(key));
        Cache.RemoveKey(Namespace, key, EntryOptions);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Asynchronously removes all values from the cache.
    /// </summary>
    public override Task RemoveAsync() => CancellationTokenSource.CancelAsync();

    /// <summary>
    /// Retrieves all keys associated with this cache.
    /// </summary>
    /// <returns>An enumerable of strings representing current cache entry keys.</returns>/// 
    public override Task<IEnumerable<string>> GetKeysAsync() =>
        Task.FromResult(Cache.GetKeys(Namespace).ToList());
}