using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using StackExchange.Redis;
using ActionCache.Hosting;
using ActionCache.Common.Extensions.Internal;

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
            .AddScoped<IActionCacheFactory, RedisActionCacheFactory>()
            .AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(options.Configuration))
            .AddHostedService<RedisHostedService>();
    }
}