using ActionCache.Caching;
using ActionCache.Common.Utilities;
using StackExchange.Redis;

namespace ActionCache.Redis;

/// <summary>
/// Factory class for creating RedisActionCache instances.
/// </summary>
public class RedisActionCacheFactory : IActionCacheFactory
{
    /// <summary>
    /// An IDatabase representation of a Redis cache.
    /// </summary> 
    protected readonly IDatabase Cache;
    protected readonly ActionCacheRefreshProvider RefreshProvider;
    
    /// <summary>
    /// Constructor for RedisActionCacheFactory.
    /// </summary>
    /// <param name="connectionMultiplexer">ConnectionMultiplexer for Redis.</param>
    public RedisActionCacheFactory(
        IConnectionMultiplexer connectionMultiplexer,
        ActionCacheRefreshProvider refreshProvider
    ) 
    {
        Cache = connectionMultiplexer.GetDatabase();
        RefreshProvider = refreshProvider;
    }
    
    /// <summary>
    /// Gets the type of cache.
    /// </summary>
    public CacheType Type => CacheType.Redis;
    
    /// <summary>
    /// Creates a new RedisActionCache instance.
    /// </summary>
    /// <param name="namespace">Namespace for the cache.</param>
    /// <returns>New instance of RedisActionCache.</returns>
    public IActionCache? Create(string @namespace) => new RedisActionCache(@namespace, Cache, RefreshProvider);
}