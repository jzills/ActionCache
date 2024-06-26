using Microsoft.Extensions.Caching.Memory;

namespace ActionCache.Memory;

public class MemoryActionCacheFactory : IActionCacheFactory
{
    protected readonly IMemoryCache MemoryCache;
    protected readonly IExpirationTokenSources ExpirationTokens;
    
    public MemoryActionCacheFactory(
        IMemoryCache memoryCache,
        IExpirationTokenSources expirationTokens) => 
            (MemoryCache, ExpirationTokens) = (memoryCache, expirationTokens);

    public CacheType Type => CacheType.Memory;

    public IActionCache? Create(string @namespace)
    {
        if (ExpirationTokens.TryGetOrAdd(@namespace, out var expirationTokenSource))
        {
            return new MemoryActionCache(@namespace, MemoryCache, expirationTokenSource);
        }
        else
        {
            return default;
        }
    }
}