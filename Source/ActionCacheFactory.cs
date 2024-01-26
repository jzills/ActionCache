using Microsoft.Extensions.DependencyInjection;
using ActionCache.Enums;

namespace ActionCache;

public class ActionCacheProvider : IActionCacheProvider
{
    protected readonly IServiceProvider ServiceProvider;
    public ActionCacheProvider(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;
    public IActionCache? Create(string @namespace, ActionCacheTypes type)
    {
        var cache = type switch
        {
            ActionCacheTypes.InMemory => ServiceProvider
                .GetRequiredService<MemoryActionCacheFactory>()
                .Create(@namespace),
            ActionCacheTypes.Redis => throw new NotImplementedException(),
            _ => throw new NotImplementedException()
        };

        return cache;
    }
}