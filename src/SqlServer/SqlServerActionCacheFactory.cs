using ActionCache.Common;
using ActionCache.Common.Caching;
using ActionCache.Common.Concurrency;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace ActionCache.SqlServer;

/// <summary>
/// Represents a factory for creating SqlServer action caches.
/// </summary>
public class SqlServerActionCacheFactory : ActionCacheFactoryBase
{
    /// <summary>
    /// A SqlServer cache implementation.
    /// </summary>
    protected readonly IDistributedCache Cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServerActionCacheFactory"/> class.
    /// </summary>
    /// <param name="cache">The SqlServer cache to use.</param>
    /// <param name="entryOptions">The global entry options used for creation when expiration times are not supplied.</param>
    /// <param name="refreshProvider">The refresh provider responsible for invoking cached controller actions.</param> 
    public SqlServerActionCacheFactory(
        IDistributedCache cache,
        IOptions<ActionCacheEntryOptions> entryOptions,
        IActionCacheRefreshProvider refreshProvider
    ) : base(entryOptions, refreshProvider)
    {
        Cache = cache;
    }

    /// <inheritdoc/>
    public override IActionCache? Create(string @namespace)
    {
        var context = new ActionCacheContext<DistributedCacheLock>
        {
            Namespace = @namespace,
            EntryOptions = EntryOptions,
            RefreshProvider = RefreshProvider,
            CacheLocker = new DistributedCacheLocker(
                Cache,
                EntryOptions.LockDuration,
                EntryOptions.LockTimeout
            )
        };

        return new SqlServerActionCache(Cache, context);
    }

    /// <inheritdoc/>
    public override IActionCache? Create(string @namespace, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
    {
        var context = new ActionCacheContext<DistributedCacheLock>
        {
            Namespace = @namespace,
            EntryOptions = new ActionCacheEntryOptions
            {
                AbsoluteExpiration = absoluteExpiration,
                SlidingExpiration = slidingExpiration
            },
            RefreshProvider = RefreshProvider,
            CacheLocker = new DistributedCacheLocker(
                Cache, 
                EntryOptions.LockDuration, 
                EntryOptions.LockTimeout
            )
        };
        
        return new SqlServerActionCache(Cache, context);
    }
}