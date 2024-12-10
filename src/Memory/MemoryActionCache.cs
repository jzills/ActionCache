using ActionCache.Common;
using ActionCache.Common.Caching;
using ActionCache.Common.Concurrency;
using ActionCache.Memory.Extensions.Internal;
using ActionCache.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace ActionCache.Memory;

/// <summary>
/// Represents a memory action cache implementation.
/// </summary>
public class MemoryActionCache : ActionCacheBase
{
    /// <summary>
    /// A memory cache implementation.
    /// </summary>
    protected readonly IMemoryCache Cache;

    /// <summary>
    /// A source of tokens used to combine cache entries for actions like namespace eviction.
    /// </summary>
    protected readonly CancellationTokenSource CancellationTokenSource;

    /// <summary>
    /// Provides a thread-safe locking mechanism for synchronizing access to resources using <see cref="SemaphoreSlim"/>.
    /// </summary>
    protected readonly SemaphoreSlimLocker CacheLocker;

    /// <summary>
    /// Initializes a new instance of the MemoryActionCache class.
    /// </summary>
    /// <param name="namespace">The namespace for cache isolation.</param>
    /// <param name="cache">The memory cache instance.</param>
    /// <param name="cancellationTokenSource">The source for cancellation tokens used to expire cache entries.</param>
    /// <param name="entryOptions">The global entry options used for creation when expiration times are not supplied.</param> 
    /// <param name="refreshProvider">The refresh provider to handle cache refreshes.</param>
    public MemoryActionCache(
        Namespace @namespace,
        IMemoryCache cache,
        CancellationTokenSource cancellationTokenSource,
        ActionCacheEntryOptions entryOptions,
        IActionCacheRefreshProvider refreshProvider
    ) : base(@namespace, entryOptions, refreshProvider)
    {
        Cache = cache;
        CancellationTokenSource = cancellationTokenSource;
        EntryOptions = new MemoryCacheEntryOptions 
        { 
            Size = 1,
            SlidingExpiration  = base.EntryOptions.SlidingExpiration,
            AbsoluteExpiration = base.EntryOptions.GetAbsoluteExpirationFromUtcNow() 
        }.AddExpirationToken(new CancellationChangeToken(CancellationTokenSource.Token));
        CacheLocker = new SemaphoreSlimLocker(
            base.EntryOptions.LockDuration, 
            base.EntryOptions.LockTimeout
        );
    }

    /// <summary>
    /// Gets the entry options for memory cache.
    /// </summary>
    /// <value>The cache entry options applied to new entries.</value>
    public new MemoryCacheEntryOptions EntryOptions { get; init; }

    /// <summary>
    /// Asynchronously gets a value from the cache.
    /// </summary>
    /// <param name="key">The key of the cache entry.</param>
    /// <returns>The cached value or null if not found.</returns> 
    public override Task<TValue> GetAsync<TValue>(string key) =>
        Task.FromResult(Cache.Get<TValue>(Namespace.Create(key)));

    /// <summary>
    /// Asynchronously sets a value in the cache.
    /// </summary>
    /// <param name="key">The cache key to set the value for.</param>
    /// <param name="value">The value to set in the cache.</param>
    public override async Task SetAsync<TValue>(string key, TValue value)
    {
        Cache.Set(Namespace.Create(key), value, EntryOptions);

        await CacheLocker.WaitForLockThenAsync(Namespace,
            () => Cache.SetKey(Namespace, key, EntryOptions));
    }

    /// <summary>
    /// Asynchronously removes a value from the cache.
    /// </summary>
    /// <param name="key">The key of the cache entry to remove.</param>
    public override async Task RemoveAsync(string key)
    {
        Cache.Remove(Namespace.Create(key));

        await CacheLocker.WaitForLockThenAsync(Namespace, 
            () => Cache.RemoveKey(Namespace, key, EntryOptions));
    }

    /// <summary>
    /// Asynchronously removes all values from the cache.
    /// </summary>
    public override Task RemoveAsync() => CancellationTokenSource.CancelAsync();

    /// <summary>
    /// Retrieves all keys associated with this cache.
    /// </summary>
    /// <returns>An enumerable of strings representing current cache entry keys.</returns>
    public override async Task<IEnumerable<string>> GetKeysAsync()
    {
        var keysWithAbsoluteExpiration = await CacheLocker.WaitForLockThenAsync(Namespace,
            () => Cache.GetKeys(Namespace, EntryOptions));

        return keysWithAbsoluteExpiration?.Keys.AsEnumerable() ?? [];
    }
}