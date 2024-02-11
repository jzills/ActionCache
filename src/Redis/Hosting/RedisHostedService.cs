using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using ActionCache.Common.Extensions;
using ActionCache.Redis;
using ActionCache.Utilities;

namespace ActionCache.Hosting;

public class RedisHostedService : IHostedService
{
    protected readonly ISubscriber Subcriber;
    protected readonly IServiceProvider ServiceProvider;

    public RedisHostedService(
        IConnectionMultiplexer mult,
        IServiceProvider serviceProvider
    )
    {
        Subcriber = mult.GetSubscriber();
        ServiceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Subcriber.SubscribeAsync(
            RedisChannel.Literal(RedisActionCacheChannels.Main),
            GetChannelHandler());
    }

    public Action<RedisChannel, RedisValue> GetChannelHandler() => async (channel, value) => {
        if (!string.IsNullOrWhiteSpace(value))
        {
            var message = JsonSerializer.Deserialize<RedisChannelMessage>(value!);
            if (message is not null)
            {
                using var scope = ServiceProvider.CreateAsyncScope();
                if (scope.ServiceProvider.TryGetActionCacheFactory(CacheProvider.Memory, out var cacheFactory))
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

    private Task HandleMessageAsync(RedisChannelMessage message, IActionCache cache) =>
        message.Type switch
        {
            MessageType.Set                => cache.SetAsync   (message.Key!, message.Value),
            MessageType.RemoveByKey        => cache.RemoveAsync(message.Key!),
            MessageType.RemoveByNamespace  => cache.RemoveAsync(),
            _                               => Task.CompletedTask
        };

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Subcriber.UnsubscribeAsync(
            RedisChannel.Literal(RedisActionCacheChannels.Main));
    }
}