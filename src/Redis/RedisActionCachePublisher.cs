using System.Text.Json;
using StackExchange.Redis;
using ActionCache.Utilities;

namespace ActionCache.Redis;

public class RedisActionCachePublisher : RedisActionCache
{
    public RedisActionCachePublisher(
        RedisNamespace @namespace, 
        IDatabase cache
    ) : base(@namespace, cache)
    {
    }

    public override async Task SetAsync<TValue>(string key, TValue? value) where TValue : default
    {
        await base.SetAsync<TValue>(key, value);
        await PublishMessageAsync(
            new RedisChannelMessage(Namespace, key, value));
    }

    public override async Task RemoveAsync(string key)
    {
        await base.RemoveAsync(key);
        await PublishMessageAsync(
            new RedisChannelMessage(Namespace, key, MessageType.RemoveByKey));
    }

    public override async Task RemoveAsync()
    {
        await base.RemoveAsync();
        await PublishMessageAsync(
            new RedisChannelMessage(Namespace, Type: MessageType.RemoveByNamespace));
    }

    private Task PublishMessageAsync(RedisChannelMessage message) => 
        Cache.PublishAsync(
            RedisChannel.Literal(RedisActionCacheChannels.Main), 
            JsonSerializer.Serialize(message));
}