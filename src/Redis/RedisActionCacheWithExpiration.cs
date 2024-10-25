using ActionCache.Common;
using ActionCache.Common.Caching;
using ActionCache.Common.Serialization;
using ActionCache.Redis.Extensions;
using ActionCache.Redis.Extensions.Internal;
using StackExchange.Redis;
using System.Reflection;

namespace ActionCache.Redis;

/// <summary>
/// Represents a Redis implementation of the IActionCache interface.
/// </summary>
public class RedisActionCacheWithExpiration : ActionCacheBase
{
    /// <summary>
    /// An IDatabase representation of a Redis cache.
    /// </summary> 
    protected readonly IDatabase Cache;

    protected new RedisNamespace Namespace => (RedisNamespace)base.Namespace;

    /// <summary>
    /// Initializes a new instance of the RedisActionCache class with the specified RedisNamespace and IDatabase.
    /// </summary>
    /// <param name="namespace">The RedisNamespace to use for caching.</param>
    /// <param name="cache">The IDatabase to use for caching.</param>
    public RedisActionCacheWithExpiration(
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
    public Assembly Assembly => typeof(RedisActionCacheWithExpiration).Assembly;

    /// <summary>
    /// Removes the cached item with the specified key.
    /// </summary>
    /// <param name="key">The key of the item to remove from the cache.</param>
    public override async Task RemoveAsync(string key)
    {
        if (Assembly.TryGetResourceAsText(LuaResources.Remove, out var script))
        {
            await Cache.ScriptEvaluateAsync(script, [Namespace, key], flags: CommandFlags.FireAndForget);
        }
        else
        {
            var isSuccessful = await Cache.KeyDeleteAsync(Namespace.Create(key));
            if (isSuccessful)
            {
                await Cache.SortedSetRemoveAsync(Namespace, key);
            }
        }
    }

    /// <summary>
    /// Removes all items in the cache associated with the current namespace.
    /// </summary>
    public override async Task RemoveAsync()
    {
        if (Assembly.TryGetResourceAsText(LuaResources.RemoveNamespace, out var script))
        {
            await Cache.ScriptEvaluateAsync(script, [Namespace], flags: CommandFlags.FireAndForget);
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

        var absoluteExpiration = EntryOptions.GetAbsoluteExpirationFromUtcNowInMilliseconds();
        var slidingExpiration = EntryOptions.GetSlidingExpirationInMilliseconds();
        var ttl = slidingExpiration > 0 ?
            slidingExpiration :
            EntryOptions.GetAbsoluteExpirationAsTTLInMilliseconds();

        if (Assembly.TryGetResourceAsText(LuaResources.SetHash, out var script))
        {
            await Cache.ScriptEvaluateAsync(script, 
                [Namespace, (RedisKey)key], 
                [redisValue, absoluteExpiration, slidingExpiration, ttl], 
                CommandFlags.FireAndForget
            );
        }
        else
        {
            await Cache.HashSetAsync(Namespace.Create(key), 
            [
                new HashEntry(RedisHashEntries.Value, redisValue),
                new HashEntry(RedisHashEntries.AbsoluteExpiration, absoluteExpiration),
                new HashEntry(RedisHashEntries.SlidingExpiration, slidingExpiration)
            ]);

            if (EntryOptions.SlidingExpiration.HasValue || EntryOptions.AbsoluteExpiration.HasValue)
            {
                await Cache.KeyExpireAsync(Namespace.Create(key), expiry: TimeSpan.FromMilliseconds(ttl));
            }

            await Cache.SortedSetAddAsync(Namespace, key, absoluteExpiration, CommandFlags.FireAndForget);
        }
    }

    /// <summary>
    /// Gets a cached item of type TValue with the specified key.
    /// </summary>
    /// <param name="key">The key of the item to retrieve from the cache.</param>
    /// <returns>The cached item if found, otherwise default.</returns>
    public override async Task<TValue> GetAsync<TValue>(string key)
    {
        var namespaceKey = Namespace.Create(key);
        var hashEntries = await Cache.HashGetAllAsync(namespaceKey);
        if (hashEntries is null || hashEntries.Length == 0)
        {
            await Cache.SortedSetRemoveAsync(Namespace, key);
            return default;
        }

        var absoluteExpirationUnix = hashEntries.GetAbsoluteExpiration();
        if (absoluteExpirationUnix > ActionCacheEntryOptions.NoExpiration)
        {
            var absoluteExpiration = DateTimeOffset.FromUnixTimeMilliseconds(absoluteExpirationUnix);
            if (DateTimeOffset.UtcNow >= absoluteExpiration)
            {
                await Cache.KeyDeleteAsync(namespaceKey);
                await Cache.SortedSetRemoveAsync(Namespace, key);
                return default;
            }
        }
        
        var slidingExpiration = hashEntries.GetSlidingExpiration();
        if (slidingExpiration > ActionCacheEntryOptions.NoExpiration)
        {
            await Cache.KeyExpireAsync(namespaceKey, TimeSpan.FromMilliseconds(slidingExpiration), CommandFlags.FireAndForget);
        }

        var jsonValue = hashEntries.GetRedisValue();
        if (string.IsNullOrWhiteSpace(jsonValue))
        {
            return default;
        }
        else
        {
            return CacheJsonSerializer.Deserialize<TValue>(jsonValue);
        }
    }

    public override async Task<IEnumerable<string>> GetKeysAsync()
    {
        await Cache.SortedSetRemoveRangeByScoreAsync(
            Namespace, 
            ActionCacheEntryOptions.NoExpiration,
            DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), 
            Exclude.Start
        );

        var entries = await Cache.SortedSetRangeByRankAsync(Namespace);
        return (IEnumerable<string>)entries.Select(value => (string?)value);
    }
}