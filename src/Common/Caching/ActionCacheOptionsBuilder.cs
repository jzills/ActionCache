namespace ActionCache.Common;

public class ActionCacheOptionsBuilder
{
    protected readonly ActionCacheOptions Options = new();
    public ActionCacheOptionsBuilder UseEntryOptions(Action<ActionCacheEntryOptions> configureOptions)
    {
        configureOptions.Invoke(Options.EntryOptions);
        return this;
    }

    public ActionCacheOptionsBuilder UseMemoryCache()
    {
        Options.EnabledCaches[CacheProvider.Memory] = true;
        return this;
    }

    public ActionCacheOptionsBuilder UseRedisCache()
    {
        Options.EnabledCaches[CacheProvider.Redis] = true;
        return this;
    }

    public ActionCacheOptionsBuilder UseSqlServerCache()
    {
        Options.EnabledCaches[CacheProvider.SqlServer] = true;
        return this;
    }

    public ActionCacheOptions Build() => Options;
}