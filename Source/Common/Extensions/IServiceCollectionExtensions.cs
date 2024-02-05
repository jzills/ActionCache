using Microsoft.Extensions.DependencyInjection;

namespace ActionCache.Common.Extensions;

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