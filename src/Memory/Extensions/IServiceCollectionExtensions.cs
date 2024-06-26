using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using ActionCache.Common.Extensions;

namespace ActionCache.Memory.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddActionCacheMemory(
        this IServiceCollection services
    ) => services.AddActionCacheMemoryInternal();

    public static IServiceCollection AddActionCacheMemory(
        this IServiceCollection services,
        Action<MemoryCacheOptions> configureOptions
    ) => services
            .AddActionCacheMemory()
            .AddMemoryCache(configureOptions);

    internal static IServiceCollection AddActionCacheMemoryInternal(
        this IServiceCollection services
    ) => services
            .AddActionCacheCommon()
            .AddExpirationTokenSources()
            .AddScoped<IActionCacheFactory, MemoryActionCacheFactory>();

    internal static IServiceCollection AddExpirationTokenSources(
        this IServiceCollection services
    ) => services.AddScoped<IExpirationTokenSources, ExpirationTokenSourcesValidated>(serviceProvider =>
            new ExpirationTokenSourcesValidated(
                new ExpirationTokenSources(
                    serviceProvider.GetRequiredService<IMemoryCache>())));
}