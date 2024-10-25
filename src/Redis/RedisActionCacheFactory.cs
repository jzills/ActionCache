using ActionCache.Common;
using ActionCache.Common.Caching;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace ActionCache.Redis;

/// <summary>
/// Factory class for creating RedisActionCache instances.
/// </summary>
public class RedisActionCacheFactory : ActionCacheFactoryBase
{
    /// <summary>
    /// An IDatabase representation of a Redis cache.
    /// </summary> 
    protected readonly IDatabase Cache;
    
    /// <summary>
    /// Constructor for RedisActionCacheFactory.
    /// </summary>
    /// <param name="connectionMultiplexer">ConnectionMultiplexer for Redis.</param>
    public RedisActionCacheFactory(
        IConnectionMultiplexer connectionMultiplexer,
        IOptions<ActionCacheEntryOptions> entryOptions,
        ActionCacheRefreshProvider refreshProvider
    ) : base(CacheType.Redis, entryOptions.Value, refreshProvider)
    {
        Cache = connectionMultiplexer.GetDatabase();
    }
    
    /// <inheritdoc/>
    public override IActionCache? Create(string @namespace) => new RedisActionCacheWithExpiration(@namespace, Cache, EntryOptions, RefreshProvider);

    /// <inheritdoc/>
    public override IActionCache? Create(string @namespace, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
    {
        var entryOptions = new ActionCacheEntryOptions
        {
            AbsoluteExpiration = absoluteExpiration,
            SlidingExpiration = slidingExpiration
        };

        return new RedisActionCacheWithExpiration(@namespace, Cache, entryOptions, RefreshProvider);
    }
}