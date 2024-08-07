using ActionCache.Common.Extensions.Internal;
using ActionCache.Common.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace ActionCache.Common.Extensions;

internal static class IServiceCollectionExtensions
{
    internal static IServiceCollection AddActionCache(
        this IServiceCollection services,
        Action<ActionCacheOptionsBuilder> configureOptions
    )
    {
        var optionsBuilder = new ActionCacheOptionsBuilder();
        configureOptions.Invoke(optionsBuilder);

        var options = optionsBuilder.Build();
        if (options.EnabledCaches[CacheType.Memory])
        {
            // Register MemoryCache dependencies
        }

        if (options.EnabledCaches[CacheType.Redis])
        {
            // Register Redis dependencies
        }

        if (options.EnabledCaches[CacheType.SqlServer])
        {
            // Register SqlServer dependencies
        }

        return services;
    }

    internal static IServiceCollection AddActionCacheCommon(
        this IServiceCollection services
    ) => services
            .AddControllerInfo()
            .AddScoped<ActionCacheDescriptorProvider>();
}