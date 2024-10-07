using ActionCache.Caching;
using StackExchange.Redis;

namespace ActionCache.Redis;

public class RedisActionCacheRefreshable : RedisActionCache
{
    public RedisActionCacheRefreshable(
        RedisNamespace @namespace, 
        IDatabase cache, 
        ActionCacheRefreshProvider refreshProvider
    ) : base(@namespace, cache, refreshProvider)
    {
    }

    public override Task SetAsync<TValue>(string key, TValue value)
    {
        return base.SetAsync(key, value);
    }

    public override Task RemoveAsync(string key)
    {
        return base.RemoveAsync(key);
    }
}