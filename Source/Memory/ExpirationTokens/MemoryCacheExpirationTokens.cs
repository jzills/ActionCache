using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace ActionCache.Memory;

public class MemoryCacheExpirationTokens
{
    protected readonly IMemoryCache Cache;
    public MemoryCacheExpirationTokens(IMemoryCache cache) => Cache = cache;
    public bool TryGetOrAdd(string key, out CancellationTokenSource cancellationTokenSource)
    {
        if (!string.IsNullOrWhiteSpace(key))
        {
            var cacheEntry = Cache.GetOrCreate(key, entry =>
            {
                var _cancellationTokenSource = new CancellationTokenSource();
                entry.Value = _cancellationTokenSource;
                entry.AddExpirationToken(new CancellationChangeToken(_cancellationTokenSource.Token));
                return entry;
            });

            var isSuccessful = 
                cacheEntry is not null && 
                cacheEntry.Value is not null;

            cancellationTokenSource = (CancellationTokenSource)(cacheEntry?.Value ?? default!);
            
            return isSuccessful;
        }
        else
        {
            cancellationTokenSource = default!;
            return false; 
        }
    }
}