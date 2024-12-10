using ActionCache.Common.Serialization;
using ActionCache.Utilities;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Concurrent;

namespace ActionCache.Memory.Extensions.Internal;

internal static class IDistributedCacheExtensions
{
    internal static async Task<ConcurrentDictionary<string, DateTimeOffset?>> GetKeysAsync(this IDistributedCache cache, Namespace @namespace, DistributedCacheEntryOptions entryOptions)
    {
        var json = cache.GetString(@namespace);
        if (string.IsNullOrWhiteSpace(json))
        {
            return new();
        }
        else
        {
            var keys = CacheJsonSerializer.Deserialize<ConcurrentDictionary<string, DateTimeOffset?>>(json);
            if (keys is null)
            {
                await cache.RemoveAsync(@namespace);
            }
            else
            {
                var entries = keys.Where(key => DateTimeOffset.UtcNow >= key.Value);
                if (entries.Any())
                {
                    foreach (var entry in entries)
                    {
                        keys.TryRemove(entry.Key, out _);
                    }

                    await cache.SetStringAsync(@namespace, CacheJsonSerializer.Serialize(keys), entryOptions);
                }
            }

            return keys ?? [];
        }
    }

    internal static async Task SetKeyAsync(this IDistributedCache cache, Namespace @namespace, string key, DistributedCacheEntryOptions entryOptions)
    {
        var keys = await cache.GetKeysAsync(@namespace, entryOptions);
        if (keys.TryAdd(key, entryOptions.AbsoluteExpiration))
        {
            await cache.SetStringAsync(@namespace, CacheJsonSerializer.Serialize(keys), entryOptions);
        }
    }

    internal static async Task RemoveKeyAsync(this IDistributedCache cache, Namespace @namespace, string key, DistributedCacheEntryOptions entryOptions)
    {
        var keys = await cache.GetKeysAsync(@namespace, entryOptions);
        if (keys.Any())
        {
            if (keys.TryRemove(key, out _))
            {
                await cache.SetStringAsync(@namespace, CacheJsonSerializer.Serialize(keys), entryOptions);
            }
        }
    }
}