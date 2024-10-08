using ActionCache.Utilities;
using Microsoft.Extensions.Caching.Memory;

namespace ActionCache.Memory.Extensions.Internal;

internal static class IMemoryCacheExtensions
{
    internal static ConcurrentHashSet<string> GetKeys(this IMemoryCache cache, Namespace @namespace)
    {
        var keys = cache.GetOrCreate(@namespace, options => {
            options.Size = 1;
            return new ConcurrentHashSet<string>();
        });

        ArgumentNullException.ThrowIfNull(keys, nameof(keys));

        return keys;
    }

    internal static void SetKey(this IMemoryCache cache, Namespace @namespace, string key, MemoryCacheEntryOptions entryOptions)
    {
        var keys = cache.GetKeys(@namespace);
        keys.Add(key);
        cache.Set(key, keys, entryOptions);
    }

    internal static void RemoveKey(this IMemoryCache cache, Namespace @namespace, string key, MemoryCacheEntryOptions entryOptions)
    {
        var keys = cache.GetKeys(@namespace);
        if (keys.Count > 0)
        {
            keys.Remove(key);
            cache.Set(@namespace, keys, entryOptions);
        }
    }
}