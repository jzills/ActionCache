using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using StackExchange.Redis;
using ActionCache.Redis;

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

        ArgumentNullException.ThrowIfNullOrWhiteSpace(options.Configuration);

        return services
            .AddStackExchangeRedisCache(configureOptions)
            .AddScoped<RedisActionCacheFactory>()
            .AddSingleton<IConnectionMultiplexer>( _ => ConnectionMultiplexer.Connect(options.Configuration));
    }
}