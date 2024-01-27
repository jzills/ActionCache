using ActionCache.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace ActionCache;

public class MemoryActionCacheFactory
{
    protected readonly IMemoryCache MemoryCache;
    protected readonly MemoryCacheExpirationTokens ExpirationTokens;
    
    public MemoryActionCacheFactory(
        IMemoryCache memoryCache,
        MemoryCacheExpirationTokens expirationTokens
    ) => (MemoryCache, ExpirationTokens) = (memoryCache, expirationTokens);

    public IActionCache? Create(string @namespace)
    {
        if (ExpirationTokens.TryGetOrAdd(@namespace, out var cancellationTokenSource))
        {
            return new MemoryActionCache(@namespace, MemoryCache, cancellationTokenSource);
        }
        else
        {
            return default;
        }
    }
}