using ActionCache.Common.Concurrency;
using ActionCache.Utilities;

namespace ActionCache.Common.Caching;

/// <summary>
/// An abstract class implementation of <see cref="IActionCache"/> 
/// </summary>
public abstract class ActionCacheBase<TLock> : IActionCache where TLock : CacheLock
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
    protected readonly IActionCacheRefreshProvider RefreshProvider;

    /// <summary>
    /// The cache locker handling operations with race conditions.
    /// </summary>
    protected readonly ICacheLocker<TLock> CacheLocker;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionCacheBase{TLock}"/> class.
    /// </summary>
    /// <param name="context">The context containing necessary dependencies for cache operations.</param>
    public ActionCacheBase(ActionCacheContext<TLock> context)
    {
        Namespace = context.Namespace;
        EntryOptions = context.EntryOptions;
        RefreshProvider = context.RefreshProvider;
        CacheLocker = context.CacheLocker;
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
        var refreshResults = RefreshProvider.GetRefreshResults(Namespace.Value, keys);
        await Task.WhenAll(refreshResults.Select(result => 
            SetAsync(result.Key, result.Value)));
    }

    /// <inheritdoc/>
    public abstract Task RemoveAsync(string key);

    /// <inheritdoc/>
    public abstract Task RemoveAsync();

    /// <inheritdoc/>
    public abstract Task SetAsync<TValue>(string key, TValue? value);
}