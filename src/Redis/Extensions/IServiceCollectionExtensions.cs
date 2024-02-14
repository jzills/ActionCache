using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using StackExchange.Redis;
using ActionCache.Hosting;
using ActionCache.Common.Extensions.Internal;
using ActionCache.Common;
using ActionCache.Common.Utilities;

namespace ActionCache.Redis.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddActionCacheRedis(
        this IServiceCollection services, 
        Action<RedisCacheOptions> configureOptions
    )
    {
        var options = new RedisCacheOptions();
        configureOptions.Invoke(options);

        ArgumentException.ThrowIfNullOrWhiteSpace(options.Configuration);

        return services
            .AddControllerInfo()
            .AddStackExchangeRedisCache(configureOptions)
            .AddScoped<ActionCacheDescriptorProvider>()
            .AddScoped<IActionCacheFactory, RedisActionCacheFactory>()
            .AddScoped<IActionCacheRehydrator, RedisActionCacheRehydrator>()
            .AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(options.Configuration))
            .AddHostedService<RedisHostedService>();
    }
}