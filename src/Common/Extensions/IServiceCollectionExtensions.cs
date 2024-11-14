using ActionCache.Common.Caching;
using ActionCache.Common.Extensions.Internal;
using ActionCache.Common.Filters;
using ActionCache.Memory.Extensions;
using ActionCache.Redis.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace ActionCache.Common.Extensions;

/// <summary>
/// Extension methods for IServiceCollection to support ActionCache.
/// </summary>
internal static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds ActionCache services to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="configureOptions">A delegate to configure ActionCacheOptions.</param>
    /// <returns>The IServiceCollection.</returns>
    internal static IServiceCollection AddActionCache(
        this IServiceCollection services,
        Action<ActionCacheOptionsBuilder> configureOptions
    )
    {
        var optionsBuilder = new ActionCacheOptionsBuilder();
        configureOptions.Invoke(optionsBuilder);

        var options = optionsBuilder.Build();
        services.Configure<ActionCacheEntryOptions>(configureOptions => 
        {
            configureOptions.SlidingExpiration = options.EntryOptions.SlidingExpiration;
            configureOptions.AbsoluteExpiration = options.EntryOptions.AbsoluteExpiration;
        });

        if (options.EnabledCaches[CacheType.Memory])
        {
            services.AddActionCacheMemory(options.ConfigureMemoryCacheOptions);
        }

        if (options.EnabledCaches[CacheType.Redis])
        {
            services.AddActionCacheRedis(options.ConfigureRedisCacheOptions);
        }

        if (options.EnabledCaches[CacheType.SqlServer])
        {
            // Register SqlServer dependencies
        }

        return services;
    }

    /// <summary>
    /// Adds common ActionCache-related services to the IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <returns>The IServiceCollection with common ActionCache services added.</returns>
    internal static IServiceCollection AddActionCacheCommon(
        this IServiceCollection services
    ) => services
            .AddControllerInfo()
            .AddScoped<IActionCacheFilterAbstractFactory, ActionCacheFilterAbstractFactory>()
            .AddScoped<IActionCacheDescriptorProvider, ActionCacheDescriptorProvider>()
            .AddScoped<IActionCacheRefreshProvider, ActionCacheRefreshProvider>();
}