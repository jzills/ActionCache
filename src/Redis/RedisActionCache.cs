using System.Reflection;
using System.Text.Json;
using StackExchange.Redis;
using ActionCache.Redis.Extensions;

namespace ActionCache.Redis;

public class RedisActionCache : IActionCache
{
    protected readonly RedisNamespace Namespace;
    protected readonly IDatabase Cache;

    public RedisActionCache(
        RedisNamespace @namespace, 
        IDatabase cache
    )
    {
        Namespace = @namespace;
        Cache = cache;
    }

    public Assembly Assembly => typeof(RedisActionCache).Assembly;

    public virtual async Task RemoveAsync(string key)
    {
        if (Assembly.TryGetResourceAsText("UnlinkKeyWithKeySet.lua", out var script))
        {
            await Cache.ScriptEvaluateAsync(script, [Namespace, key], null, CommandFlags.FireAndForget);
        }
        else
        {
            var isSuccessful = await Cache.KeyDeleteAsync(Namespace.Create(key));
            if (isSuccessful)
            {
                await Cache.SetRemoveAsync(Namespace, key);
            }
        }
    }

    public virtual async Task RemoveAsync()
    {
        if (Assembly.TryGetResourceAsText("UnlinkNamespaceWithKeySet.lua", out var script))
        {
            await Cache.ScriptEvaluateAsync(script, [Namespace], null, CommandFlags.FireAndForget);
        }
    }

    public virtual async Task SetAsync<TValue>(string key, TValue? value)
    {
        RedisValue redisValue = JsonSerializer.Serialize(value);

        if (Assembly.TryGetResourceAsText("SetJsonWithKeySet.lua", out var script))
        {
            await Cache.ScriptEvaluateAsync(script, 
                [Namespace, (RedisKey)key], 
                [redisValue], 
                CommandFlags.FireAndForget
            );
        }
        else
        {
            var isSuccessful = await Cache.StringSetAsync(Namespace.Create(key), redisValue);
            if (isSuccessful)
            {
                await Cache.SetAddAsync(Namespace, key, CommandFlags.FireAndForget);
            }
        }
    }

    public async Task<TValue?> GetAsync<TValue>(string key)
    {
        var value = await Cache.StringGetAsync(Namespace.Create(key));
        if (!string.IsNullOrWhiteSpace(value))
        {
            return JsonSerializer.Deserialize<TValue>(value!);
        }
        else
        {
            return default;
        }
    }
}