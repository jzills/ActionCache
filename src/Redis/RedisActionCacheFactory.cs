using StackExchange.Redis;

namespace ActionCache.Redis;

public class RedisActionCacheFactory : IActionCacheFactory
{
    protected readonly IDatabase Cache;
    public RedisActionCacheFactory(IConnectionMultiplexer connectionMultiplexer) => Cache = connectionMultiplexer.GetDatabase();
    public CacheType Type => CacheType.Redis;
    public IActionCache? Create(string @namespace) => new RedisActionCache(@namespace, Cache);//new RedisActionCachePublisher(@namespace, Cache);
}