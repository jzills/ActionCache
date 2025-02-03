using ActionCache.Common;
using ActionCache.Common.Caching;
using ActionCache.Common.Concurrency.Locks;
using ActionCache.Common.Serialization;
using ActionCache.Redis.Extensions;
using ActionCache.Redis.Extensions.Internal;
using StackExchange.Redis;
using System.Reflection;

namespace ActionCache.Redis;

/// <summary>
/// Represents a Redis implementation of the IActionCache interface.
/// </summary>
public class RedisActionCache : ActionCacheBase<NullCacheLock>
{
    /// <summary>
    /// An IDatabase representation of a Redis cache.
    /// </summary> 
    protected readonly IDatabase Cache;

    /// <summary>
    /// The namespace used for cache entries.
    /// </summary>
    //protected new RedisNamespace Namespace => (RedisNamespace)base.Namespace;

    /// <summary>
    /// Initializes a new instance of the RedisActionCache class with the specified RedisNamespace and IDatabase.
    /// </summary>
    /// <param name="cache">The IDatabase to use for caching.</param>
    /// <param name="context">The contextual information.</param> 
    public RedisActionCache(IDatabase cache, ActionCacheContext<NullCacheLock> context) : base(context)
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
        if (Assembly.TryGetResourceAsText(LuaResources.Remove, out var script))
        {
            await Cache.ScriptEvaluateAsync(script, [(string)Namespace, key], flags: CommandFlags.FireAndForget);
        }
        else
        {
            var isSuccessful = await Cache.KeyDeleteAsync(Namespace.Create(key));
            if (isSuccessful)
            {
                await Cache.SortedSetRemoveAsync((string)Namespace, key);
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
            await Cache.ScriptEvaluateAsync(script, [(string)Namespace], flags: CommandFlags.FireAndForget);
        }
    }

#pragma warning disable CS8765

    /// <summary>
    /// Sets a cached item with the specified key and value.
    /// </summary>
    /// <param name="key">The key of the item to set in the cache.</param>
    /// <param name="value">The value of the item to set in the cache.</param>
    public override async Task SetAsync<TValue>(string key, TValue value)
    {
        RedisValue redisValue = CacheJsonSerializer.Serialize(value);

        var (absoluteExpiration, slidingExpiration, ttl) = EntryOptions;
        
        if (Assembly.TryGetResourceAsText(LuaResources.SetHash, out var script))
        {
            await Cache.ScriptEvaluateAsync(script, 
                [(string)Namespace, (RedisKey)key], 
                [redisValue, absoluteExpiration, slidingExpiration, ttl], 
                CommandFlags.FireAndForget
            );
        }
        else
        {
            await Cache.HashSetAsync(Namespace.Create(key), 
            [
                new HashEntry(RedisHashEntry.Value, redisValue),
                new HashEntry(RedisHashEntry.AbsoluteExpiration, absoluteExpiration),
                new HashEntry(RedisHashEntry.SlidingExpiration, slidingExpiration)
            ]);

            if (EntryOptions.SlidingExpiration.HasValue || EntryOptions.AbsoluteExpiration.HasValue)
            {
                await Cache.KeyExpireAsync(Namespace.Create(key), expiry: TimeSpan.FromMilliseconds(ttl));
            }

            await Cache.SortedSetAddAsync((string)Namespace, key, absoluteExpiration, CommandFlags.FireAndForget);
        }
    }

#pragma warning restore CS8765

#pragma warning disable CS8609, CS8603

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
            await Cache.SortedSetRemoveAsync((string)Namespace, key);
            return default;
        }

        var absoluteExpirationUnix = hashEntries.GetAbsoluteExpiration();
        if (absoluteExpirationUnix > ActionCacheEntryOptions.NoExpiration)
        {
            var absoluteExpiration = DateTimeOffset.FromUnixTimeMilliseconds(absoluteExpirationUnix);
            if (DateTimeOffset.UtcNow >= absoluteExpiration)
            {
                await Cache.KeyDeleteAsync(namespaceKey);
                await Cache.SortedSetRemoveAsync((string)Namespace, key);
                return default;
            }
        }
        
        var slidingExpiration = hashEntries.GetSlidingExpiration();
        if (slidingExpiration > ActionCacheEntryOptions.NoExpiration)
        {
            await Cache.KeyExpireAsync(namespaceKey, TimeSpan.FromMilliseconds(slidingExpiration), CommandFlags.FireAndForget);
        }

        var jsonValue = (string?)hashEntries.GetRedisValue();
        if (string.IsNullOrWhiteSpace(jsonValue))
        {
            return default;
        }
        else
        {
            return CacheJsonSerializer.Deserialize<TValue>(jsonValue);
        }
    }

#pragma warning restore CS8609, CS8603

    /// <inheritdoc/>
    public override async Task<IEnumerable<string>> GetKeysAsync()
    {
        await Cache.SortedSetRemoveRangeByScoreAsync(
            (string)Namespace, 
            ActionCacheEntryOptions.NoExpiration,
            DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), 
            Exclude.Start
        );

        var entries = await Cache.SortedSetRangeByRankAsync((string)Namespace);
        return entries.Select(value => (string)value!);
    }
}