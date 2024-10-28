
using ActionCache.Utilities;

namespace ActionCache.Common.Caching;

public class ActionCacheHandler : ActionCacheHandlerBase, IActionCache
{
    protected readonly IActionCache Cache;
    
    public ActionCacheHandler(IActionCache cache) => Cache = cache;

    public async Task<TValue?> GetAsync<TValue>(string key)
    {
        var result = await Cache.GetAsync<TValue?>(key);
        if (result is not null)
        {
            return result;
        }
        else if (Next is not null)
        {
            return await Next.GetAsync<TValue?>(key);
        }
        else
        {
            return default;
        }
    }

    public async Task<IEnumerable<string>> GetKeysAsync()
    {
        var result = await Cache.GetKeysAsync();
        if (result is not null)
        {
            return result;
        }
        else if (Next is not null)
        {
            return await Next.GetKeysAsync();
        }
        else
        {
            return [];
        }
    }

    public Namespace GetNamespace() => Cache.GetNamespace();

    public async Task RefreshAsync()
    {
        await Cache.RefreshAsync();

        if (Next is not null)
        {
            await Next.RefreshAsync();
        }
    }

    public async Task RemoveAsync(string key)
    {
        await Cache.RemoveAsync(key);

        if (Next is not null)
        {
            await Next.RemoveAsync(key);
        }
    }

    public async Task RemoveAsync()
    {
        await Cache.RemoveAsync();

        if (Next is not null)
        {
            await Next.RemoveAsync();
        }
    }

    public async Task SetAsync<TValue>(string key, TValue? value)
    {
        await Cache.SetAsync(key, value);

        if (Next is not null)
        {
            await Next.SetAsync(key, value);
        }
    }

    public IActionCache SetNext(IActionCache next)
    {
        Next = next;
        return next;
    }
}