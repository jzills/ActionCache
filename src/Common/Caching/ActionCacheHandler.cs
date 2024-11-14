using ActionCache.Utilities;

namespace ActionCache.Common.Caching;

/// <summary>
/// Provides an implementation of <see cref="IActionCacheHandler"/> that handles action caching with support for chaining multiple caches.
/// </summary>
public class ActionCacheHandler : ActionCacheHandlerBase, IActionCache
{
    /// <summary>
    /// The primary <see cref="IActionCache"/> instance used for caching operations.
    /// </summary>
    protected readonly IActionCache Cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionCacheHandler"/> class with the specified cache instance.
    /// </summary>
    /// <param name="cache">The <see cref="IActionCache"/> instance used for caching operations.</param>
    public ActionCacheHandler(IActionCache cache) => Cache = cache;

    /// <summary>
    /// Retrieves an item from the cache by key. If the item is not found in this cache, attempts to retrieve it from the next cache in the chain.
    /// </summary>
    /// <typeparam name="TValue">The type of the cached value.</typeparam>
    /// <param name="key">The key of the cached item.</param>
    /// <returns>The cached value if found; otherwise, <c>null</c>.</returns>
    public async Task<TValue?> GetAsync<TValue>(string key) =>
        await Cache.GetAsync<TValue?>(key) ?? 
            await NextIfExists(next => next.GetAsync<TValue?>(key));

    /// <summary>
    /// Retrieves all cache keys available in this cache. If no keys are found, attempts to retrieve keys from the next cache in the chain.
    /// </summary>
    /// <returns>An enumerable of cache keys.</returns>
    public async Task<IEnumerable<string>> GetKeysAsync() =>
        await Cache.GetKeysAsync() ?? 
            await NextIfExists(next => next.GetKeysAsync()) ?? [];

    /// <summary>
    /// Gets the namespace associated with this cache.
    /// </summary>
    /// <returns>The cache namespace.</returns>
    public Namespace GetNamespace() => Cache.GetNamespace();

    /// <summary>
    /// Refreshes the cache, potentially updating or reloading cached entries. Also refreshes the next cache in the chain, if it exists.
    /// </summary>
    /// <returns>A task that represents the asynchronous refresh operation.</returns>
    public async Task RefreshAsync()
    {
        await Cache.RefreshAsync();
        await NextIfExists(next => next.RefreshAsync());
    }

    /// <summary>
    /// Removes a specific item from the cache by key. Also removes the item from the next cache in the chain, if it exists.
    /// </summary>
    /// <param name="key">The key of the item to remove.</param>
    /// <returns>A task that represents the asynchronous remove operation.</returns>
    public async Task RemoveAsync(string key)
    {
        await Cache.RemoveAsync(key);
        await NextIfExists(next => next.RemoveAsync(key));
    }

    /// <summary>
    /// Removes all items from the cache. Also removes all items from the next cache in the chain, if it exists.
    /// </summary>
    /// <returns>A task that represents the asynchronous remove operation.</returns>
    public async Task RemoveAsync()
    {
        await Cache.RemoveAsync();
        await NextIfExists(next => next.RemoveAsync());
    }

    /// <summary>
    /// Sets a value in the cache with the specified key. Also sets the value in the next cache in the chain, if it exists.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to cache.</typeparam>
    /// <param name="key">The key for the cached value.</param>
    /// <param name="value">The value to cache.</param>
    /// <returns>A task that represents the asynchronous set operation.</returns>
    public async Task SetAsync<TValue>(string key, TValue? value)
    {
        await Cache.SetAsync(key, value);
        await NextIfExists(next => next.SetAsync(key, value));
    }
}