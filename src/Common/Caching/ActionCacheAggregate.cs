namespace ActionCache;

/// <summary>
/// Represents a combined action cache that aggregates multiple caches.
/// </summary>
public class ActionCacheAggregate : IActionCache
{
    /// <summary>
    /// Stores a collection of IActionCache instances.
    /// </summary>
    protected readonly IEnumerable<IActionCache> Caches;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionCacheAggregate"/> class.
    /// </summary>
    /// <param name="caches">The collection of caches to aggregate.</param>
    public ActionCacheAggregate(IEnumerable<IActionCache> caches) => Caches = caches;

    /// <summary>
    /// Asynchronously retrieves the specified cache value.
    /// </summary>
    /// <typeparam name="TValue">The type of the cached value.</typeparam>
    /// <param name="key">The key of the cache value.</param>
    /// <returns>The cached value if found; otherwise, null.</returns>
    public async Task<TValue?> GetAsync<TValue>(string key)
    {
        TValue? value = default;
        foreach (var cache in Caches)
        {
            value = await cache.GetAsync<TValue>(key);
            if (value is not null)
            {
                break;
            }
        }

        return value;
    }

    public async Task<IEnumerable<string>> GetKeysAsync()
    {
        var keys = new List<string>();
        foreach (var cache in Caches)
        {
            keys.AddRange(await cache.GetKeysAsync());
        }

        return keys;
    }

    public async Task RefreshAsync()
    {
        foreach (var cache in Caches)
        {
            await cache.RefreshAsync();
        }
    }

    /// <summary>
    /// Asynchronously removes the specified key from the cache.
    /// </summary>
    /// <param name="key">The key to remove.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task RemoveAsync(string key) =>
        WhenAllAggregate(cache => cache.RemoveAsync(key));

    /// <summary>
    /// Asynchronously removes all keys from the cache.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task RemoveAsync() =>
        WhenAllAggregate(cache => cache.RemoveAsync());

    /// <summary>
    /// Asynchronously sets the value of the specified key in the cache.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to cache.</typeparam>
    /// <param name="key">The key to set.</param>
    /// <param name="value">The value to set.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task SetAsync<TValue>(string key, TValue? value) =>
        WhenAllAggregate(cache => cache.SetAsync(key, value));

    /// <summary>
    /// Executes a task on all aggregated caches and waits for all to complete.
    /// </summary>
    /// <param name="cacheSelector">The function to execute on each cache.</param>
    /// <returns>A task representing the asynchronous operation of all tasks.</returns>
    private Task WhenAllAggregate(Func<IActionCache, Task> cacheSelector) =>
        Task.WhenAll(Caches.Select(cacheSelector));
}