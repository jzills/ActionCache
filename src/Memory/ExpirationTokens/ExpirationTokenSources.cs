using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace ActionCache.Memory;

public class ExpirationTokenSources : IExpirationTokenSources
{
    protected readonly IMemoryCache Cache;
    public ExpirationTokenSources(IMemoryCache cache) => Cache = cache;

    public MemoryCacheEntryOptions EntryOptions(CancellationTokenSource source) => 
            new MemoryCacheEntryOptions { Size = 1 }
                .AddExpirationToken(
                    new CancellationChangeToken(source.Token));

    public bool TryGetOrAdd(string key, out CancellationTokenSource cancellationTokenSource)
    {
        if (!Cache.TryGetValue(key, out cancellationTokenSource!))
        {
            cancellationTokenSource ??= new CancellationTokenSource();
            Cache.Set(key, cancellationTokenSource, EntryOptions(cancellationTokenSource));
        }
        
        return true;
    }
}