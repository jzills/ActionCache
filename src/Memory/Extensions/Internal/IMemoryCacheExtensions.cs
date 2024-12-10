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

        if (keys is null)
        {
            cache.Remove(@namespace);
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

                cache.Set(@namespace, keys, entryOptions);
            }
        }

        return keys ?? [];
    }

    internal static void SetKey(this IMemoryCache cache, Namespace @namespace, string key, MemoryCacheEntryOptions entryOptions)
    {
        var keys = cache.GetKeys(@namespace, entryOptions);
        if (keys.TryAdd(key, entryOptions.AbsoluteExpiration))
        {
            cache.Set(key, keys, entryOptions);
        }
    }

    internal static void RemoveKey(this IMemoryCache cache, Namespace @namespace, string key, MemoryCacheEntryOptions entryOptions)
    {
        var keys = cache.GetKeys(@namespace, entryOptions);
        if (keys.Any())
        {
            if (keys.TryRemove(key, out _))
            {
                cache.Set(@namespace, keys, entryOptions);
            }
        }
    }
}