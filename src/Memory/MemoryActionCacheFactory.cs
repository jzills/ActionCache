using ActionCache.Common;
using ActionCache.Common.Caching;
using ActionCache.Common.Concurrency;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace ActionCache.Memory;

/// <summary>
/// Represents a factory for creating memory action caches.
/// </summary>
public class MemoryActionCacheFactory : ActionCacheFactoryBase
{
    /// <summary>
    /// A memory cache implementation.
    /// </summary>
    protected readonly IMemoryCache Cache;

    /// <summary>
    /// A source of expiration tokens.
    /// </summary>
    protected readonly IExpirationTokenSources ExpirationTokens;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryActionCacheFactory"/> class.
    /// </summary>
    /// <param name="cache">The memory cache to use.</param>
    /// <param name="expirationTokens">The expiration token source to use.</param>
    /// <param name="entryOptions">The global entry options used for creation when expiration times are not supplied.</param>
    /// <param name="refreshProvider">The refresh provider responsible for invoking cached controller actions.</param> 
    public MemoryActionCacheFactory(
        IMemoryCache cache,
        IExpirationTokenSources expirationTokens,
        IOptions<ActionCacheEntryOptions> entryOptions,
        IActionCacheRefreshProvider refreshProvider
    ) : base(entryOptions, refreshProvider)
    {
        Cache = cache;
        ExpirationTokens = expirationTokens;
    }

    /// <inheritdoc/>
    public override IActionCache? Create(string @namespace)
    {
        if (ExpirationTokens.TryGetOrAdd(@namespace, out var expirationTokenSource))
        {
            var context = new ActionCacheContext<SemaphoreSlimLock>
            {
                Namespace = @namespace,
                EntryOptions = EntryOptions,
                RefreshProvider = RefreshProvider,
                CacheLocker = new SemaphoreSlimLocker(
                    EntryOptions.LockDuration, 
                    EntryOptions.LockTimeout
                )
            };

            return new MemoryActionCache(Cache, expirationTokenSource, context);
        }
        else
        {
            return default;
        }
    }

    /// <inheritdoc/>
    public override IActionCache? Create(string @namespace, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
    {
        if (ExpirationTokens.TryGetOrAdd(@namespace, out var expirationTokenSource))
        {
            var context = new ActionCacheContext<SemaphoreSlimLock>
            {
                Namespace = @namespace,
                EntryOptions = new ActionCacheEntryOptions
                {
                    AbsoluteExpiration = absoluteExpiration,
                    SlidingExpiration = slidingExpiration
                },
                RefreshProvider = RefreshProvider,
                CacheLocker = new SemaphoreSlimLocker(
                    EntryOptions.LockDuration, 
                    EntryOptions.LockTimeout
                )
            };

            return new MemoryActionCache(Cache, expirationTokenSource, context);
        }
        else
        {
            return default;
        }
    }
}