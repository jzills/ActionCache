using Microsoft.Extensions.DependencyInjection;

namespace ActionCache.Common.Extensions;

public class ActionCacheBuilder
{
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

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddActionCache(
        this IServiceCollection services,
        Action<ActionCacheBuilder> configureOptions
    )
    {
        return services;
    }
}