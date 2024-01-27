using Microsoft.Extensions.DependencyInjection;
using ActionCache.Enums;
using ActionCache.Redis;

namespace ActionCache;

public class ActionCacheProvider : IActionCacheProvider
{
    protected readonly IServiceProvider ServiceProvider;
    public ActionCacheProvider(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;
    public IActionCache? Create(string @namespace, ActionCacheTypes type)
    {
        var cache = type switch
        {
            ActionCacheTypes.InMemory   => GetMemoryCache(@namespace),
            ActionCacheTypes.Redis      => GetRedisCache (@namespace),
            _                           => throw new NotImplementedException()
        };

        return new ActionCacheValidated(cache);
    }

    private IActionCache? GetMemoryCache(string @namespace) => 
        ServiceProvider
            .GetRequiredService<MemoryActionCacheFactory>()
            .Create(@namespace);

    private IActionCache? GetRedisCache(string @namespace) =>
        ServiceProvider
            .GetRequiredService<RedisActionCacheFactory>()
            .Create(@namespace);
}