using System.Text.Json;
using StackExchange.Redis;
using ActionCache.Utilities;

namespace ActionCache.Redis;

/// <summary>
/// Represents a Redis-based implementation of an action cache for publishing events.
/// </summary>
public class RedisActionCachePublisher : RedisActionCache
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RedisActionCachePublisher"/> class.
    /// </summary>
    /// <param name="namespace">The Redis namespace.</param>
    /// <param name="cache">The Redis cache database.</param>
    public RedisActionCachePublisher(
        RedisNamespace @namespace, 
        IDatabase cache
    ) : base(@namespace, cache)
    {
    }

    /// <summary>
    /// Asynchronously sets a value in the cache and publishes a message for the key.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to be set.</typeparam>
    /// <param name="key">The key for the cache entry.</param>
    /// <param name="value">The value to be set.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task SetAsync<TValue>(string key, TValue? value) where TValue : default
    {
        await base.SetAsync(key, value);
        await PublishMessageAsync(
            new RedisChannelMessage(Namespace, key, value));
    }

    /// <summary>
    /// Asynchronously removes a value from the cache and publishes a message for the key.
    /// </summary>
    /// <param name="key">The key for the cache entry.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task RemoveAsync(string key)
    {
        await base.RemoveAsync(key);
        await PublishMessageAsync(
            new RedisChannelMessage(Namespace, key, MessageType.RemoveByKey));
    }

    /// <summary>
    /// Asynchronously removes all values from the cache under the namespace and publishes a message.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task RemoveAsync()
    {
        await base.RemoveAsync();
        await PublishMessageAsync(
            new RedisChannelMessage(Namespace, Type: MessageType.RemoveByNamespace));
    }

    /// <summary>
    /// Asynchronously publishes a message to the Redis channel.
    /// </summary>
    /// <param name="message">The Redis channel message to be published.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private Task PublishMessageAsync(RedisChannelMessage message) => 
        Cache.PublishAsync(
            RedisChannel.Literal(RedisActionCacheChannels.Main), 
            JsonSerializer.Serialize(message));
}