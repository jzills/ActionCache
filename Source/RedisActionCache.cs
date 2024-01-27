using System.Text.Json;
using StackExchange.Redis;

namespace ActionCache.Redis;

public class RedisActionCache : IActionCache
{
    protected readonly string Namespace;
    protected readonly IDatabase Cache;

    public RedisActionCache(
        string @namespace,
        IDatabase cache
    ) 
    {
        Namespace = @namespace;
        Cache = cache;
    }

    public Task<TValue?> GetAsync<TValue>(string key)
    {
        var jsonValue = Cache.StringGet($"{Namespace}:{key}");
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
        return Cache.KeyDeleteAsync($"{Namespace}:{key}");
    }

    public Task RemoveAsync()
    {
        //Cache.ScriptEvaluateAsync();
        throw new NotImplementedException();
    }

    public Task SetAsync<TValue>(string key, TValue? value)
    {
        return Cache.StringSetAsync($"{Namespace}:{key}", JsonSerializer.Serialize(value));
    }
}