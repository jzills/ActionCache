using Microsoft.Extensions.Caching.Memory;
using ActionCache.Utilities;

namespace ActionCache.Memory;

public class MemoryActionCacheFactory : IActionCacheFactory
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
            return new MemoryActionCache(new Namespace(@namespace), MemoryCache, cancellationTokenSource);
        }
        else
        {
            return default;
        }
    }
}