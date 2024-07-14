using ActionCache.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using StackExchange.Redis;

namespace ActionCache.Redis.Extensions;

/// <summary>
/// Extension methods for adding ActionCache with Redis to the IServiceCollection.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds ActionCache with Redis to the IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <returns>The updated IServiceCollection.</returns>
    public static IServiceCollection AddActionCacheRedis(
        this IServiceCollection services
    ) => services.AddActionCacheRedisInternal();

    /// <summary>
    /// Adds ActionCache with Redis to the IServiceCollection with custom configuration options.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <param name="configureOptions">An Action to configure the RedisCacheOptions.</param>
    /// <returns>The updated IServiceCollection.</returns>
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

    /// <summary>
    /// Internal method to add ActionCache with Redis services to the IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <returns>The updated IServiceCollection.</returns>
    internal static IServiceCollection AddActionCacheRedisInternal(
        this IServiceCollection services
    ) => services
            .AddActionCacheCommon()
            .AddScoped<IActionCacheFactory, RedisActionCacheFactory>()
            .AddHostedService<RedisHostedService>();
}