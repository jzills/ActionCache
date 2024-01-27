namespace ActionCache;

public class ActionCacheValidated : IActionCache
{
    protected readonly IActionCache Cache;

    public ActionCacheValidated(IActionCache cache) => Cache = cache;

    public Task<TValue?> GetAsync<TValue>(string key)
    {
        return Cache.GetAsync<TValue?>(key);
    }

    public Task RemoveAsync(string key)
    {
        return Cache.RemoveAsync(key);
    }

    public Task RemoveAsync()
    {
        return Cache.RemoveAsync();
    }

    public Task SetAsync<TValue>(string key, TValue? value)
    {
        return Cache.SetAsync(key, value);
    }
}