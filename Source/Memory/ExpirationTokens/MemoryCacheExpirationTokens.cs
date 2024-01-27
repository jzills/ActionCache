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
            if (!Cache.TryGetValue(key, out cancellationTokenSource!))
            {
                cancellationTokenSource = new CancellationTokenSource();
                var options = new MemoryCacheEntryOptions { Size = 1 };
                options.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                Cache.Set(key, cancellationTokenSource, options);
            }
            
            return true;
        }
        else
        {
            cancellationTokenSource = default!;
            return false; 
        }
    }
}