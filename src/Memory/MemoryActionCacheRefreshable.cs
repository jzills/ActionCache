using ActionCache.Caching;
using ActionCache.Utilities;
using Microsoft.Extensions.Caching.Memory;

namespace ActionCache.Memory;

public class MemoryActionCacheRefreshable : MemoryActionCache
{
    public MemoryActionCacheRefreshable(
        Namespace @namespace, 
        IMemoryCache cache, 
        CancellationTokenSource cancellationTokenSource, 
        ActionCacheRefreshProvider refreshProvider
    ) : base(@namespace, cache, cancellationTokenSource, refreshProvider)
    {
    }

    public override Task SetAsync<TValue>(string key, TValue value)
    {
        var keys = Cache.GetOrCreate(Namespace, options => {
            options.Size = 1;
            return new ConcurrentHashSet<string>();
        });

        keys?.Add(key);
        Cache.Set(key, keys, EntryOptions);

        return base.SetAsync(key, value);
    }

    public override Task RemoveAsync(string key)
    {
        if (Cache.TryGetValue<ConcurrentHashSet<string>>(Namespace, out var keys))
        {
            if (keys?.Count > 0)
            {
                keys.Remove(key);
                Cache.Set(Namespace, keys, EntryOptions);
            }
        }
        
        return base.RemoveAsync(key);
    }
}