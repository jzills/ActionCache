namespace ActionCache;

/// <summary>
/// Provides an interface for caching actions in a key-value store.
/// </summary>
public interface IActionCache
{
    /// <summary>
    /// Retrieves the value associated with the specified key from the cache.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to retrieve.</typeparam>
    /// <param name="key">The key of the value to retrieve.</param>
    /// <returns>The value associated with the specified key, or null if the key does not exist.</returns>
    Task<TValue?> GetAsync<TValue>(string key);

    /// <summary>
    /// Sets the value with the specified key in the cache.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to set.</typeparam>
    /// <param name="key">The key of the value to store.</param>
    /// <param name="value">The value to store. Can be null.</param>
    /// <returns>A task that represents the asynchronous set operation.</returns>
    Task SetAsync<TValue>(string key, TValue? value);

    /// <summary>
    /// Removes the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to remove.</param>
    /// <returns>A task that represents the asynchronous remove operation.</returns>
    Task RemoveAsync(string key);

    /// <summary>
    /// Removes all values and keys from the cache.
    /// </summary>
    /// <returns>A task that represents the asynchronous remove operation.</returns>
    Task RemoveAsync();
}