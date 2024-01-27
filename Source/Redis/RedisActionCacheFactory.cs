using StackExchange.Redis;
using ActionCache.Utilities;

namespace ActionCache.Redis;

public class RedisActionCacheFactory : IActionCacheFactory
{
    protected readonly IDatabase Cache;
    public RedisActionCacheFactory(IConnectionMultiplexer mult) => Cache = mult.GetDatabase();
    public IActionCache? Create(string @namespace) =>
        new RedisActionCache(new Namespace(@namespace), Cache);
}