using Microsoft.Extensions.Caching.Memory;

namespace ActionCache.Common;

public class ActionCacheEntryOptions
{
    public TimeSpan? AbsoluteExpiration { get; set; }
    public TimeSpan? SlidingExpiration { get; set; }
}

public class ActionCacheBuilder
{
    public ActionCacheBuilder UseEntryOptions(Action<ActionCacheEntryOptions> configureOptions)
    {
        return this;
    }

    public ActionCacheBuilder UseMemoryCache()
    {
        return this;
    }

    public ActionCacheBuilder UseRedisCache()
    {
        return this;
    }

    public ActionCacheBuilder UseSqlServerCache()
    {
        return this;
    }
}