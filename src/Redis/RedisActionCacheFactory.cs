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
    /// <param name="entryOptions">The global entry options used for creation when expiration times are not supplied.</param> 
    /// <param name="refreshProvider">The refresh provider to handle cache refreshes.</param>  
    public RedisActionCacheFactory(
        IConnectionMultiplexer connectionMultiplexer,
        IOptions<ActionCacheEntryOptions> entryOptions,
        IActionCacheRefreshProvider refreshProvider
    ) : base(entryOptions.Value, refreshProvider)
    {
        Cache = connectionMultiplexer.GetDatabase();
    }
    
    /// <inheritdoc/>
    public override IActionCache? Create(string @namespace) => 
        new RedisActionCache(@namespace, Cache, EntryOptions, RefreshProvider);

    /// <inheritdoc/>
    public override IActionCache? Create(string @namespace, 
        TimeSpan? absoluteExpiration = null, 
        TimeSpan? slidingExpiration = null
    )
    {
        var entryOptions = new ActionCacheEntryOptions
        {
            AbsoluteExpiration = absoluteExpiration,
            SlidingExpiration = slidingExpiration
        };

        return new RedisActionCache(@namespace, Cache, entryOptions, RefreshProvider);
    }
}