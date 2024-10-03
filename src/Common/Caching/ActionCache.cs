using ActionCache.Caching;
using ActionCache.Utilities;

namespace ActionCache.Common.Caching;

public abstract class ActionCache : IActionCache
{
    protected readonly Namespace Namespace;
    protected readonly ActionCacheRefreshProvider RefreshProvider;
    public ActionCache(Namespace @namespace, ActionCacheRefreshProvider refreshProvider)
    {
        Namespace = @namespace;
        RefreshProvider = refreshProvider;
    }

    public abstract Task<TValue?> GetAsync<TValue>(string key);

    public Task<IEnumerable<string>> GetKeysAsync()
    {
        throw new NotImplementedException();
    }

    public Namespace GetNamespace() => Namespace;

    public async Task RefreshAsync()
    {
        var keys = await GetKeysAsync();
        var refreshResults = await RefreshProvider.GetRefreshResultsAsync(Namespace.Value, keys);

        var refreshTasks = new List<Task>();
        foreach (var refreshResult in refreshResults)
        {
            refreshTasks.Add(SetAsync(refreshResult.Key, refreshResult.Value));
        }

        await Task.WhenAll(refreshTasks);
    }

    public abstract Task RemoveAsync(string key);
    public abstract Task RemoveAsync();
    public abstract Task SetAsync<TValue>(string key, TValue? value);

}