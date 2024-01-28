using System.Text.Json;
using System.Reflection;
using StackExchange.Redis;
using ActionCache.Utilities;
using ActionCache.Redis.Extensions;

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

    public Task RemoveAsync(string key) =>
        Cache.KeyDeleteAsync(Namespace.Create(key));

    public async Task RemoveAsync()
    {
        var assembly = Assembly.GetExecutingAssembly();
        if (assembly.TryGetResourceAsText("UnlinkNamespace.lua", out var script))
        {
            await Cache.ScriptEvaluateAsync(script, null, new[] 
            { 
                new RedisValue($"{Namespace.@namespace}:*") 
            });
        }
        else
        {
            // TODO: Use IConnectionMultiplexer to get all servers,
            // iterate through servers + scan keys and remove 
            // namespaces as fallback
            // REF: https://stackexchange.github.io/StackExchange.Redis/KeysScan.html 
        }
    }

    public Task SetAsync<TValue>(string key, TValue? value) =>
        Cache.StringSetAsync(
            Namespace.Create(key), 
            JsonSerializer.Serialize(value));
}