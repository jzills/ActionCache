namespace ActionCache;

public class ActionCacheAggregate : IActionCache
{
    protected readonly IEnumerable<IActionCache> Caches;
    public ActionCacheAggregate(IEnumerable<IActionCache> caches) => Caches = caches;

    public async Task<TValue?> GetAsync<TValue>(string key)
    {
        TValue? value = default;
        foreach (var cache in Caches)
        {
            value = await cache.GetAsync<TValue>(key);
            if (value is not null) 
            {
                break;
            }
        }

        return value;
    }

    public Task RemoveAsync(string key) =>
        WhenAllAggregate(cache => cache.RemoveAsync(key));

    public Task RemoveAsync() =>
        WhenAllAggregate(cache => cache.RemoveAsync());

    public Task SetAsync<TValue>(string key, TValue? value) =>
        WhenAllAggregate(cache => cache.SetAsync(key, value));
        
    private Task WhenAllAggregate(Func<IActionCache, Task> cacheSelector) => 
        Task.WhenAll(Caches.Select(cacheSelector));
}