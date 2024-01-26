using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace ActionCache;

public class MemoryActionCache : IActionCache
{
    protected readonly string Namespace;
    protected readonly IMemoryCache Cache;
    protected readonly CancellationTokenSource CancellationTokenSource;

    public MemoryActionCache(
        string @namespace,
        IMemoryCache cache,
        CancellationTokenSource cancellationTokenSource
    ) 
    {
        Namespace = @namespace;
        Cache = cache;
        CancellationTokenSource = cancellationTokenSource;
    }

    public Task<TValue?> GetAsync<TValue>(string key)
    {
        return Task.FromResult(Cache.Get<TValue?>($"{Namespace}:{key}"));
    }

    public Task SetAsync<TValue>(string key, TValue? value)
    {
        var options = new MemoryCacheEntryOptions();
        options.ExpirationTokens.Add(
            new CancellationChangeToken(CancellationTokenSource.Token));
        
        Cache.Set<TValue?>($"{Namespace}:{key}", value, options);

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        Cache.Remove($"{Namespace}:{key}");
        return Task.CompletedTask;
    }

    public Task RemoveAsync()
    {
        CancellationTokenSource.Cancel();
        return Task.CompletedTask;
    }
}