using ActionCache.Common.Extensions;
using ActionCache.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System.Text.Json;

namespace ActionCache.Redis;

/// <summary>
/// Implements the IHostedService interface to create a Redis hosted service that subscribes to a Redis channel
/// and processes incoming messages.
/// </summary>
public class RedisHostedService : IHostedService
{
    /// <summary>
    /// A Redis subscriber.
    /// </summary> 
    protected readonly ISubscriber Subcriber;

    /// <summary>
    /// A service provider to request injected services.
    /// </summary> 
    protected readonly IServiceProvider ServiceProvider;

    /// <summary>
    /// Initializes a new instance of the RedisHostedService class.
    /// </summary>
    /// <param name="connectionMultiplexer">The connection multiplexer used to connect to Redis.</param>
    /// <param name="serviceProvider">The service provider for dependency injection.</param>
    public RedisHostedService(
        IConnectionMultiplexer connectionMultiplexer,
        IServiceProvider serviceProvider
    )
    {
        Subcriber = connectionMultiplexer.GetSubscriber();
        ServiceProvider = serviceProvider;
    }

    /// <summary>
    /// Starts the Redis hosted service and subscribes to the main Redis channel.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task StartAsync(CancellationToken cancellationToken) =>
        await Subcriber.SubscribeAsync(
            RedisChannel.Literal(RedisActionCacheChannels.Main),
            GetChannelHandler());

    /// <summary>
    /// Gets the channel handler action for processing incoming messages.
    /// </summary>
    /// <returns>The channel handler action.</returns>
    public Action<RedisChannel, RedisValue> GetChannelHandler() => async (channel, value) => {
        if (!string.IsNullOrWhiteSpace(value))
        {
            var message = JsonSerializer.Deserialize<RedisChannelMessage>(value!);
            if (message is not null)
            {
                using var scope = ServiceProvider.CreateAsyncScope();
                if (scope.ServiceProvider.TryGetActionCacheFactory(CacheType.Memory, out var cacheFactory))
                {
                    var cache = cacheFactory.Create(message.Namespace);
                    if (cache is not null)
                    {
                        await HandleMessageAsync(message, cache); 
                    }
                }
            }
        }
    };

    /// <summary>
    /// Handles a RedisChannelMessage by performing actions based on the message type.
    /// </summary>
    /// <param name="message">The RedisChannelMessage to handle.</param>
    /// <param name="cache">The IActionCache instance for caching operations.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private Task HandleMessageAsync(RedisChannelMessage message, IActionCache cache) =>
        message.Type switch
        {
            MessageType.Set                => cache.SetAsync   (message.Key!, message.Value),
            MessageType.RemoveByKey        => cache.RemoveAsync(message.Key!),
            MessageType.RemoveByNamespace  => cache.RemoveAsync(),
            _                              => Task.CompletedTask
        };

    /// <summary>
    /// Stops the Redis hosted service by unsubscribing from the main Redis channel.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task StopAsync(CancellationToken cancellationToken) =>
        await Subcriber.UnsubscribeAsync(
            RedisChannel.Literal(RedisActionCacheChannels.Main));
}