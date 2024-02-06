using System.Text.Json;
using ActionCache.Utilities;
using StackExchange.Redis;

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
        
        var message = new RedisChannelMessage(Namespace, key, value);
        await PublishMessageAsync(message);
    }

    public override async Task RemoveAsync(string key)
    {
        await base.RemoveAsync(key);

        var message = new RedisChannelMessage(Namespace, key, MessageTypes.RemoveByKey);
        await PublishMessageAsync(message);
    }

    public override async Task RemoveAsync()
    {
        await base.RemoveAsync();

        var message = new RedisChannelMessage(Namespace, Type: MessageTypes.RemoveByNamespace);
        await PublishMessageAsync(message);
    }

    private Task PublishMessageAsync(RedisChannelMessage message) => 
        Cache.PublishAsync(
            RedisChannel.Literal(RedisActionCacheChannels.Main), 
            JsonSerializer.Serialize(message));
}