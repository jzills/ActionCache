using ActionCache.Utilities;

namespace ActionCache.Common.Caching;

public abstract class ActionCacheBase : IActionCache
{
    protected readonly Namespace Namespace;
    protected readonly ActionCacheEntryOptions EntryOptions;
    protected readonly ActionCacheRefreshProvider RefreshProvider;

    public ActionCacheBase(
        Namespace @namespace, 
        ActionCacheEntryOptions entryOptions,
        ActionCacheRefreshProvider refreshProvider
    )
    {
        Namespace = @namespace;
        EntryOptions = entryOptions;
        RefreshProvider = refreshProvider;
    }

    public abstract Task<TValue?> GetAsync<TValue>(string key);

    public abstract Task<IEnumerable<string>> GetKeysAsync();

    public Namespace GetNamespace() => Namespace;

    public async Task RefreshAsync()
    {
        var keys = await GetKeysAsync();
        var refreshResults = await RefreshProvider.GetRefreshResultsAsync(Namespace.Value, keys);

        var refreshTasks = new List<Task>();
        foreach (var (key, value) in refreshResults)
        {
            refreshTasks.Add(SetAsync(key, value));
        }

        await Task.WhenAll(refreshTasks);
    }

    public abstract Task RemoveAsync(string key);

    public abstract Task RemoveAsync();

    public abstract Task SetAsync<TValue>(string key, TValue? value);
}