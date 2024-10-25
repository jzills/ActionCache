using ActionCache.Common;
using ActionCache.Common.Caching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace ActionCache.Memory;

/// <summary>
/// Represents a factory for creating memory action caches.
/// </summary>
public class MemoryActionCacheFactory : ActionCacheFactoryBase
{
    protected readonly IMemoryCache MemoryCache;
    protected readonly IExpirationTokenSources ExpirationTokens;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryActionCacheFactory"/> class.
    /// </summary>
    /// <param name="memoryCache">The memory cache to use.</param>
    /// <param name="expirationTokens">The expiration token source to use.</param>
    /// <param name="entryOptions">The global entry options used for creation when expiration times are not supplied.</param>
    /// <param name="refreshProvider">The refresh provider responsible for invoking cached controller actions.</param> 
    public MemoryActionCacheFactory(
        IMemoryCache memoryCache,
        IExpirationTokenSources expirationTokens,
        IOptions<ActionCacheEntryOptions> entryOptions,
        ActionCacheRefreshProvider refreshProvider
    ) : base(CacheType.Memory, entryOptions.Value, refreshProvider)
    {
        MemoryCache = memoryCache;
        ExpirationTokens = expirationTokens;
    }

    /// <inheritdoc/>
    public override IActionCache? Create(string @namespace)
    {
        if (ExpirationTokens.TryGetOrAdd(@namespace, out var expirationTokenSource))
        {
            return new MemoryActionCache(@namespace, MemoryCache, expirationTokenSource, EntryOptions, RefreshProvider);
        }
        else
        {
            return default;
        }
    }

    /// <inheritdoc/>
    public override IActionCache? Create(string @namespace, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration)
    {
        if (ExpirationTokens.TryGetOrAdd(@namespace, out var expirationTokenSource))
        {
            var entryOptions = new ActionCacheEntryOptions
            {
                AbsoluteExpiration = absoluteExpiration,
                SlidingExpiration = slidingExpiration
            };
            
            return new MemoryActionCache(@namespace, MemoryCache, expirationTokenSource, entryOptions, RefreshProvider);
        }
        else
        {
            return default;
        }
    }
}