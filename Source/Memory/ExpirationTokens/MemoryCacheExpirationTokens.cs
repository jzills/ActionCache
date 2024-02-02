using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace ActionCache.Memory;

public class MemoryCacheExpirationTokens
{
    protected readonly IMemoryCache Cache;
    public MemoryCacheExpirationTokens(IMemoryCache cache) => Cache = cache;

    public MemoryCacheEntryOptions EntryOptions(CancellationTokenSource source) => 
            new MemoryCacheEntryOptions { Size = 1 }
                .AddExpirationToken(
                    new CancellationChangeToken(source.Token));

    public bool TryGetOrAdd(string key, out CancellationTokenSource cancellationTokenSource)
    {
        if (!string.IsNullOrWhiteSpace(key))
        {
            if (!Cache.TryGetValue(key, out cancellationTokenSource!))
            {
                Cache.Set(key, cancellationTokenSource, EntryOptions(cancellationTokenSource));
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