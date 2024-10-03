using ActionCache.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace ActionCache.Memory;

/// <summary>
/// Represents a factory for creating memory action caches.
/// </summary>
public class MemoryActionCacheFactory : IActionCacheFactory
{
    protected readonly IMemoryCache MemoryCache;
    protected readonly IExpirationTokenSources ExpirationTokens;
    protected readonly ActionCacheRefreshProvider RefreshProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryActionCacheFactory"/> class.
    /// </summary>
    /// <param name="memoryCache">The memory cache to use.</param>
    /// <param name="expirationTokens">The expiration token source to use.</param>
    public MemoryActionCacheFactory(
        IMemoryCache memoryCache,
        IExpirationTokenSources expirationTokens) => 
            (MemoryCache, ExpirationTokens) = (memoryCache, expirationTokens);

    /// <summary>
    /// Gets the type of cache.
    /// </summary>
    public CacheType Type => CacheType.Memory;

    /// <summary>
    /// Creates an action cache for the specified namespace.
    /// </summary>
    /// <param name="namespace">The namespace for the cache.</param>
    /// <returns>A new action cache if successful, otherwise null.</returns>
    public IActionCache? Create(string @namespace)
    {
        if (ExpirationTokens.TryGetOrAdd(@namespace, out var expirationTokenSource))
        {
            return new MemoryActionCache(@namespace, MemoryCache, expirationTokenSource, RefreshProvider);
        }
        else
        {
            return default;
        }
    }
}