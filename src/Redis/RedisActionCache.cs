using ActionCache.Common;
using ActionCache.Common.Caching;
using ActionCache.Common.Serialization;
using ActionCache.Redis.Extensions;
using ActionCache.Utilities;
using StackExchange.Redis;
using System.Reflection;

namespace ActionCache.Redis;

/// <summary>
/// Represents a Redis implementation of the IActionCache interface.
/// </summary>
public class RedisActionCache : ActionCacheBase
{
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
        IDatabase cache,
        ActionCacheEntryOptions entryOptions,
        ActionCacheRefreshProvider refreshProvider
    ) : base(@namespace, entryOptions, refreshProvider)
    {
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
    public override async Task RemoveAsync(string key)
    {
        if (Assembly.TryGetResourceAsText("UnlinkKeyWithKeySet.lua", out var script))
        {
            await Cache.ScriptEvaluateAsync(script, [(RedisNamespace)Namespace, key], null, CommandFlags.FireAndForget);
        }
        else
        {
            var isSuccessful = await Cache.KeyDeleteAsync(Namespace.Create(key));
            if (isSuccessful)
            {
                await Cache.SetRemoveAsync((RedisNamespace)Namespace, key);
            }
        }
    }

    /// <summary>
    /// Removes all items in the cache associated with the current namespace.
    /// </summary>
    public override async Task RemoveAsync()
    {
        if (Assembly.TryGetResourceAsText("UnlinkNamespaceWithKeySet.lua", out var script))
        {
            await Cache.ScriptEvaluateAsync(script, [(RedisNamespace)Namespace], null, CommandFlags.FireAndForget);
        }
    }

    /// <summary>
    /// Sets a cached item with the specified key and value.
    /// </summary>
    /// <param name="key">The key of the item to set in the cache.</param>
    /// <param name="value">The value of the item to set in the cache.</param>
    public override async Task SetAsync<TValue>(string key, TValue value)
    {
        RedisValue redisValue = CacheJsonSerializer.Serialize(value);

        if (Assembly.TryGetResourceAsText("SetJsonWithKeySet.lua", out var script))
        {
            await Cache.ScriptEvaluateAsync(script, 
                [(RedisNamespace)Namespace, (RedisKey)key], 
                [redisValue], 
                CommandFlags.FireAndForget
            );
        }
        else
        {
            var isSuccessful = await Cache.StringSetAsync(Namespace.Create(key), redisValue);
            if (isSuccessful)
            {
                await Cache.SetAddAsync((RedisNamespace)Namespace, key, CommandFlags.FireAndForget);
            }
        }
    }

    /// <summary>
    /// Gets a cached item of type TValue with the specified key.
    /// </summary>
    /// <param name="key">The key of the item to retrieve from the cache.</param>
    /// <returns>The cached item if found, otherwise default.</returns>
    public override async Task<TValue> GetAsync<TValue>(string key)
    {
        var value = await Cache.StringGetAsync(Namespace.Create(key));
        if (!string.IsNullOrWhiteSpace(value))
        {
            return CacheJsonSerializer.Deserialize<TValue>(value);
        }
        else
        {
            return default;
        }
    }

    public override async Task<IEnumerable<string>> GetKeysAsync()
    {
        var value = await Cache.SetMembersAsync((RedisNamespace)Namespace);
        return (IEnumerable<string>)value.Select(value => (string?)value);
    }
}