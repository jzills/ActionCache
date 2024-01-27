using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using ActionCache.Memory;

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
            // .AddSingleton<ConcurrentDictionaryExpirationTokens>()
            .AddScoped<MemoryCacheExpirationTokens>()
            .AddScoped<MemoryActionCacheFactory>();
    } 
}