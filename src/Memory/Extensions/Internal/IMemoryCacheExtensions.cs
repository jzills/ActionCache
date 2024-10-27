using System.Collections.Concurrent;
using ActionCache.Utilities;
using Microsoft.Extensions.Caching.Memory;

namespace ActionCache.Memory.Extensions.Internal;

internal static class IMemoryCacheExtensions
{
    internal static ConcurrentDictionary<string, DateTimeOffset?> GetKeys(this IMemoryCache cache, Namespace @namespace, MemoryCacheEntryOptions entryOptions)
    {
        var keys = cache.GetOrCreate(@namespace, options => {
            options.Size = 1;
            return new ConcurrentDictionary<string, DateTimeOffset?>();
        });

        var expiredEntries = keys.Where(key => DateTimeOffset.UtcNow >= key.Value);
        if (expiredEntries.Any())
        {
            foreach (var entry in expiredEntries)
            {
                keys.Remove(entry.Key, out var _);
            }

            cache.Set(@namespace, keys, entryOptions);
        }

        ArgumentNullException.ThrowIfNull(keys, nameof(keys));

        return keys;
    }

    internal static void SetKey(this IMemoryCache cache, Namespace @namespace, string key, MemoryCacheEntryOptions entryOptions)
    {
        var keys = cache.GetKeys(@namespace, entryOptions);
        keys.TryAdd(key, entryOptions.AbsoluteExpiration);
        cache.Set(key, keys, entryOptions);
    }

    internal static void RemoveKey(this IMemoryCache cache, Namespace @namespace, string key, MemoryCacheEntryOptions entryOptions)
    {
        var keys = cache.GetKeys(@namespace, entryOptions);
        if (keys.Count > 0)
        {
            keys.Remove(key, out var _);
            cache.Set(@namespace, keys, entryOptions);
        }
    }
}