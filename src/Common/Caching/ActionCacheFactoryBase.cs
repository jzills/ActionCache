namespace ActionCache.Common.Caching;

/// <summary>
/// A base class for cache factories.
/// </summary>
public abstract class ActionCacheFactoryBase : IActionCacheFactory
{
    /// <summary>
    /// An instance of global entry options.
    /// </summary>
    protected readonly ActionCacheEntryOptions EntryOptions;

    /// <summary>
    /// An instance of a refresh provider responsible for invoking cached controller actions.
    /// </summary>
    protected readonly IActionCacheRefreshProvider RefreshProvider;

    /// <summary>
    /// The base constructor.
    /// </summary>
    /// <param name="entryOptions">The global entry options.</param>
    /// <param name="refreshProvider">The refresh provider.</param>
    public ActionCacheFactoryBase(
        ActionCacheEntryOptions entryOptions,
        IActionCacheRefreshProvider refreshProvider
    )
    {
        EntryOptions = entryOptions;
        RefreshProvider = refreshProvider;
    }

    /// <summary>
    /// Creates an action cache for the specified namespace.
    /// </summary>
    /// <param name="namespace">The namespace for the cache.</param>
    /// <returns>A new action cache if successful, otherwise null.</returns>
    public abstract IActionCache? Create(string @namespace);

    /// <summary>
    /// Creates an action cache for the specified namespace.
    /// </summary>
    /// <param name="namespace">The namespace for the cache.</param>
    /// <param name="absoluteExpiration">The absolute expiration used for entries on this cache.</param>
    /// <param name="slidingExpiration">The sliding expiration used for entries on this cache.</param>
    /// /// <returns>A new action cache if successful, otherwise null.</returns>
    public abstract IActionCache? Create(string @namespace, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration);
}