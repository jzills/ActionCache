using Microsoft.Extensions.DependencyInjection;

namespace ActionCache.Common.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddActionCache(
        this IServiceCollection services,
        Action<ActionCacheOptionsBuilder> configureOptions
    )
    {
        var optionsBuilder = new ActionCacheOptionsBuilder();
        configureOptions.Invoke(optionsBuilder);

        var options = optionsBuilder.Build();
        if (options.EnabledCaches[CacheProvider.Memory])
        {
            // Register MemoryCache dependencies
        }

        if (options.EnabledCaches[CacheProvider.Redis])
        {
            // Register Redis dependencies
        }

        if (options.EnabledCaches[CacheProvider.SqlServer])
        {
            // Register SqlServer dependencies
        }

        return services;
    }
}