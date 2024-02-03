using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;

namespace ActionCache.Memory.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddActionCacheMemory(
        this IServiceCollection services, 
        Action<MemoryCacheOptions> configureOptions
    )
    {
        return services
            .AddMemoryCache(configureOptions)
            .AddScoped<IExpirationTokenSources, ExpirationTokenSourcesValidated>(serviceProvider =>
            {
                var cache = serviceProvider.GetRequiredService<IMemoryCache>();
                return new ExpirationTokenSourcesValidated(
                    new ExpirationTokenSources(cache));
            })
            .AddScoped<IActionCacheFactory, MemoryActionCacheFactory>();
    } 
}