using System.Collections.Concurrent;
using ActionCache.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace ActionCache.Memory;

/// <summary>
/// Represents a memory action cache implementation.
/// </summary>
public class MemoryActionCache : IActionCache
{
    public readonly Namespace Namespace;
    protected readonly IMemoryCache Cache;
    protected readonly CancellationTokenSource CancellationTokenSource;

    /// <summary>
    /// Initializes a new instance of the MemoryActionCache class.
    /// </summary>
    /// <param name="namespace">The namespace for cache isolation.</param>
    /// <param name="cache">The memory cache instance.</param>
    /// <param name="cancellationTokenSource">The source for cancellation tokens used to expire cache entries.</param>
    public MemoryActionCache(
        Namespace @namespace,
        IMemoryCache cache,
        CancellationTokenSource cancellationTokenSource
    )
    {
        Namespace = @namespace;
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
    public Task<TValue?> GetAsync<TValue>(string key) =>
        Task.FromResult(Cache.Get<TValue?>(Namespace.Create(key)));

    /// <summary>
    /// Asynchronously sets a value in the cache.
    /// </summary>
    /// <param name="key">The cache key to set the value for.</param>
    /// <param name="value">The value to set in the cache.</param>
    public Task SetAsync<TValue>(string key, TValue? value)
    {
        Cache.Set(Namespace.Create(key), value, EntryOptions);

        // Use concurrent bag
        var keys = Cache.GetOrCreate(Namespace, options => {
            options.Size = 1;
            return new ConcurrentHashSet<string>();
        });
        
        keys.Add(key);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Asynchronously removes a value from the cache.
    /// </summary>
    /// <param name="key">The key of the cache entry to remove.</param>
    public Task RemoveAsync(string key)
    {
        Cache.Remove(Namespace.Create(key));

        var keys = (ConcurrentHashSet<string>?)Cache.Get(Namespace);
        if (keys?.Count > 0)
        {
            keys.Remove(key);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Asynchronously removes all values from the cache.
    /// </summary>
    public Task RemoveAsync() => CancellationTokenSource.CancelAsync();

    /// <summary>
    /// Retrieves all keys associated with this cache.
    /// </summary>
    /// <returns>An enumerable of strings representing current cache entry keys.</returns>/// 
    public Task<IEnumerable<string>> GetKeysAsync()
    {
        IEnumerable<string> keys = [];

        if (Cache.TryGetValue<ConcurrentHashSet<string>>(Namespace, out var keysHashSet))
        {
            keys = keysHashSet?.ToList() ?? [];
        }

        return Task.FromResult(keys);
    }

    public Task RefreshAsync()
    {
        throw new NotImplementedException();
    }

    public Namespace GetNamespace() => Namespace;
}