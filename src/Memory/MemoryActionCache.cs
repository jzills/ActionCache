using ActionCache.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace ActionCache.Memory;

/// <summary>
/// Represents a memory action cache implementation.
/// </summary>
public class MemoryActionCache : IActionCache
{
    protected readonly Namespace Namespace;
    protected readonly IMemoryCache Cache;
    protected readonly CancellationTokenSource CancellationTokenSource;

    /// <summary>
    /// Initializes a new instance of the MemoryActionCache class.
    /// </summary>
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
    public MemoryCacheEntryOptions EntryOptions =>
        new MemoryCacheEntryOptions { Size = 1 }
            .AddExpirationToken(
                new CancellationChangeToken(CancellationTokenSource.Token));

    /// <summary>
    /// Asynchronously gets a value from the cache.
    /// </summary>
    public Task<TValue?> GetAsync<TValue>(string key) =>
        Task.FromResult(Cache.Get<TValue?>(Namespace.Create(key)));

    /// <summary>
    /// Asynchronously sets a value in the cache.
    /// </summary>
    public Task SetAsync<TValue>(string key, TValue? value)
    {
        Cache.Set(Namespace.Create(key), value, EntryOptions);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Asynchronously removes a value from the cache.
    /// </summary>
    public Task RemoveAsync(string key)
    {
        Cache.Remove(Namespace.Create(key));
        return Task.CompletedTask;
    }

    /// <summary>
    /// Asynchronously removes all values from the cache.
    /// </summary>
    public Task RemoveAsync() => CancellationTokenSource.CancelAsync();
}