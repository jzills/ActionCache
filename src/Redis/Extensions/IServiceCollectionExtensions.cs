using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using StackExchange.Redis;
using ActionCache.Hosting;
using ActionCache.Common.Extensions;

namespace ActionCache.Redis.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddActionCacheRedis(
        this IServiceCollection services
    ) => services.AddActionCacheRedisInternal();

    public static IServiceCollection AddActionCacheRedis(
        this IServiceCollection services, 
        Action<RedisCacheOptions> configureOptions
    )
    {
        var options = new RedisCacheOptions();
        configureOptions.Invoke(options);

        ArgumentException.ThrowIfNullOrWhiteSpace(options.Configuration);

        return services
            .AddActionCacheRedis()
            .AddStackExchangeRedisCache(configureOptions)
            .AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(options.Configuration));
    }

    internal static IServiceCollection AddActionCacheRedisInternal(
        this IServiceCollection services
    ) => services
            .AddActionCacheCommon()
            .AddScoped<IActionCacheFactory, RedisActionCacheFactory>()
            .AddHostedService<RedisHostedService>();
}