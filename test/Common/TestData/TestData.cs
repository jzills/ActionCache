using ActionCache.Memory.Extensions;
using ActionCache.Redis.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Unit.Common.Data;

public static class TestData
{
    public static IEnumerable<IServiceProvider> GetServiceProviders()
    {
        var memoryServiceProvider = new ServiceCollection()
            .AddMemoryCache()
            .AddActionCacheMemory(options => options.SizeLimit = int.MaxValue)
            .BuildServiceProvider();

        var redisServiceProvider = new ServiceCollection()
            .AddActionCacheRedis(options => options.Configuration = "127.0.0.1:6379")
            .BuildServiceProvider();

        return [memoryServiceProvider, redisServiceProvider];
    }
}