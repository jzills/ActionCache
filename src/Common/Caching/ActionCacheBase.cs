using ActionCache.Utilities;

namespace ActionCache.Common.Caching;

/// <summary>
/// An abstract class implementation of <see cref="IActionCache"/> 
/// </summary>
public abstract class ActionCacheBase : IActionCache
{
    /// <summary>
    /// The namespace used for cache entries.
    /// </summary>
    protected readonly Namespace Namespace;
    
    /// <summary>
    /// The global entry options used for creation when expiration times are not supplied.
    /// </summary> 
    protected readonly ActionCacheEntryOptions EntryOptions;

    /// <summary>
    /// The refresh provider to handle cache refreshes.
    /// </summary>
    protected readonly ActionCacheRefreshProvider RefreshProvider;

    /// <summary>
    /// The constructor for the abstract cache base.
    /// </summary>
    /// <param name="namespace">The namespace to use for caching.</param>
    /// <param name="entryOptions">The global entry options used for creation when expiration times are not supplied.</param> 
    /// <param name="refreshProvider">The refresh provider to handle cache refreshes.</param> 
    public ActionCacheBase(
        Namespace @namespace, 
        ActionCacheEntryOptions entryOptions,
        ActionCacheRefreshProvider refreshProvider
    )
    {
        Namespace = @namespace;
        EntryOptions = entryOptions;
        RefreshProvider = refreshProvider;
    }

    /// <inheritdoc/>
    public abstract Task<TValue?> GetAsync<TValue>(string key);

    /// <inheritdoc/>
    public abstract Task<IEnumerable<string>> GetKeysAsync();

    /// <inheritdoc/>
    public Namespace GetNamespace() => Namespace;

    /// <inheritdoc/>
    public async Task RefreshAsync()
    {
        var keys = await GetKeysAsync();
        var refreshResults = await RefreshProvider.GetRefreshResultsAsync(Namespace.Value, keys);

        var refreshTasks = new List<Task>();
        foreach (var (key, value) in refreshResults)
        {
            refreshTasks.Add(SetAsync(key, value));
        }

        await Task.WhenAll(refreshTasks);
    }

    /// <inheritdoc/>
    public abstract Task RemoveAsync(string key);

    /// <inheritdoc/>
    public abstract Task RemoveAsync();

    /// <inheritdoc/>
    public abstract Task SetAsync<TValue>(string key, TValue? value);
}