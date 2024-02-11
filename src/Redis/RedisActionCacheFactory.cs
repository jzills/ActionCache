using StackExchange.Redis;

namespace ActionCache.Redis;

public class RedisActionCacheFactory : IActionCacheFactory
{
    protected readonly IDatabase Cache;
    public RedisActionCacheFactory(IConnectionMultiplexer mult) => Cache = mult.GetDatabase();
    public CacheProvider Provider => CacheProvider.Redis;
    public IActionCache? Create(string @namespace) => new RedisActionCachePublisher(@namespace, Cache);
}