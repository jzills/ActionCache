using ActionCache.Redis.Extensions;
using StackExchange.Redis;
using System.Reflection;
using System.Text.Json;

namespace ActionCache.Redis;

/// <summary>
/// Represents a Redis implementation of the IActionCache interface.
/// </summary>
public class RedisActionCache : IActionCache
{
    /// <summary>
    /// A namespace for this Redis cache.
    /// </summary>
    protected readonly RedisNamespace Namespace;

    /// <summary>
    /// An IDatabase representation of a Redis cache.
    /// </summary> 
    protected readonly IDatabase Cache;

    /// <summary>
    /// Initializes a new instance of the RedisActionCache class with the specified RedisNamespace and IDatabase.
    /// </summary>
    /// <param name="namespace">The RedisNamespace to use for caching.</param>
    /// <param name="cache">The IDatabase to use for caching.</param>
    public RedisActionCache(
        RedisNamespace @namespace, 
        IDatabase cache
    )
    {
        Namespace = @namespace;
        Cache = cache;
    }

    /// <summary>
    /// Gets the Assembly of the RedisActionCache class.
    /// </summary>
    public Assembly Assembly => typeof(RedisActionCache).Assembly;

    /// <summary>
    /// Removes the cached item with the specified key.
    /// </summary>
    /// <param name="key">The key of the item to remove from the cache.</param>
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

    /// <summary>
    /// Removes all items in the cache associated with the current namespace.
    /// </summary>
    public virtual async Task RemoveAsync()
    {
        if (Assembly.TryGetResourceAsText("UnlinkNamespaceWithKeySet.lua", out var script))
        {
            await Cache.ScriptEvaluateAsync(script, [Namespace], null, CommandFlags.FireAndForget);
        }
    }

    /// <summary>
    /// Sets a cached item with the specified key and value.
    /// </summary>
    /// <param name="key">The key of the item to set in the cache.</param>
    /// <param name="value">The value of the item to set in the cache.</param>
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

    /// <summary>
    /// Gets a cached item of type TValue with the specified key.
    /// </summary>
    /// <param name="key">The key of the item to retrieve from the cache.</param>
    /// <returns>The cached item if found, otherwise default.</returns>
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