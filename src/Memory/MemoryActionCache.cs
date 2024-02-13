using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using ActionCache.Utilities;

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

    public MemoryCacheEntryOptions EntryOptions => 
        new MemoryCacheEntryOptions { Size = 1 }
            .AddExpirationToken(
                new CancellationChangeToken(CancellationTokenSource.Token));

    public Task<TValue?> GetAsync<TValue>(string key) =>
        Task.FromResult(Cache.Get<TValue?>(Namespace.Create(key)));

    public Task SetAsync<TValue>(string key, TValue? value)
    {
        Cache.Set(Namespace.Create(key), value, EntryOptions);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        Cache.Remove(Namespace.Create(key));
        return Task.CompletedTask;
    }

    public Task RemoveAsync() => CancellationTokenSource.CancelAsync();
}