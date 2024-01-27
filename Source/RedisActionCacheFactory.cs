using StackExchange.Redis;

namespace ActionCache.Redis;

public class RedisActionCacheFactory
{
    protected readonly IDatabase Cache;
    public RedisActionCacheFactory(IConnectionMultiplexer mult) => Cache = mult.GetDatabase();
    public IActionCache? Create(string @namespace) =>
        new RedisActionCache(@namespace, Cache);
}