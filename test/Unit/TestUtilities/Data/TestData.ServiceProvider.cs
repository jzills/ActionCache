using ActionCache.Memory.Extensions;
using ActionCache.Redis.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Unit.TestUtiltiies.Data;

public static partial class TestData
{
    public static IEnumerable<IServiceProvider> GetServiceProviders() =>
        GetMemoryCacheServiceProvider().Concat(
            GetRedisCacheServiceProvider());

    public static IEnumerable<IServiceProvider> GetMemoryCacheServiceProvider()
    {
        var memoryServiceProvider = new ServiceCollection()
            .AddMemoryCache()
            .AddActionCacheMemory(options => options.SizeLimit = int.MaxValue)
            .BuildServiceProvider();

        return [memoryServiceProvider];
    }

    public static IEnumerable<IServiceProvider> GetRedisCacheServiceProvider()
    {
        var redisServiceProvider = new ServiceCollection()
            .AddActionCacheRedis(options => options.Configuration = "127.0.0.1:6379")
            .BuildServiceProvider();

        return [redisServiceProvider];
    }
}