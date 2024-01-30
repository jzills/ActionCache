using ActionCache.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace ActionCache.Memory;

public class MemoryActionCache : IActionCache
{
    protected readonly Namespace Namespace;
    protected readonly IMemoryCache Cache;
    protected readonly CancellationTokenSource CancellationTokenSource;

    public MemoryActionCache(
        Namespace @namespace,
        IMemoryCache cache,
        CancellationTokenSource cancellationTokenSource
    ) 
    {
        Namespace = @namespace;
        Cache = cache;
        CancellationTokenSource = cancellationTokenSource;
    }

    public Task<TValue?> GetAsync<TValue>(string key) =>
        Task.FromResult(Cache.Get<TValue?>(Namespace.Create(key)));

    public Task<bool> SetAsync<TValue>(string key, TValue? value)
    {
        var options = new MemoryCacheEntryOptions { Size = 1 };
        options.ExpirationTokens.Add(
            new CancellationChangeToken(CancellationTokenSource.Token));
        
        Cache.Set<TValue?>(Namespace.Create(key), value, options);

        return Task.FromResult(true);
    }

    public Task<bool> RemoveAsync(string key)
    {
        Cache.Remove(Namespace.Create(key));
        return Task.FromResult(true);
    }

    public Task<bool> RemoveAsync()
    {
        CancellationTokenSource.Cancel();
        return Task.FromResult(true);
    }
}