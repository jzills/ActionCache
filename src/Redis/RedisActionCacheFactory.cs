using ActionCache.Common;
using ActionCache.Common.Caching;
using Microsoft.Extensions.Options;
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
    protected readonly ActionCacheEntryOptions EntryOptions;
    protected readonly ActionCacheRefreshProvider RefreshProvider;
    
    /// <summary>
    /// Constructor for RedisActionCacheFactory.
    /// </summary>
    /// <param name="connectionMultiplexer">ConnectionMultiplexer for Redis.</param>
    public RedisActionCacheFactory(
        IConnectionMultiplexer connectionMultiplexer,
        IOptions<ActionCacheEntryOptions> entryOptions,
        ActionCacheRefreshProvider refreshProvider
    ) 
    {
        Cache = connectionMultiplexer.GetDatabase();
        EntryOptions = entryOptions.Value;
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
    public IActionCache? Create(string @namespace) => new RedisActionCache(@namespace, Cache, EntryOptions, RefreshProvider);
}