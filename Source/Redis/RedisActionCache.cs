using System.Text.Json;
using StackExchange.Redis;
using ActionCache.Utilities;

namespace ActionCache.Redis;

public class RedisActionCache : IActionCache
{
    protected readonly Namespace Namespace;
    protected readonly IDatabase Cache;

    public RedisActionCache(
        Namespace @namespace,
        IDatabase cache
    ) 
    {
        Namespace = @namespace;
        Cache = cache;
    }

    public Task<TValue?> GetAsync<TValue>(string key)
    {
        var jsonValue = Cache.StringGet(Namespace.Create(key));
        if (!string.IsNullOrWhiteSpace(jsonValue))
        {
            var value = JsonSerializer.Deserialize<TValue>(jsonValue!);
            return Task.FromResult(value);
        }
        else
        {
            return Task.FromResult<TValue?>(default);
        }
    }

    public Task RemoveAsync(string key)
    {
        return Cache.KeyDeleteAsync(Namespace.Create(key));
    }

    public Task RemoveAsync()
    {
        //Cache.ScriptEvaluateAsync();
        throw new NotImplementedException();
    }

    public Task SetAsync<TValue>(string key, TValue? value)
    {
        return Cache.StringSetAsync(Namespace.Create(key), JsonSerializer.Serialize(value));
    }
}