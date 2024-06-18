using ActionCache;
using ActionCache.Redis.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Unit.Redis;

[TestFixture]
public class Test_RedisActionCache_RemoveAsync_Namespace
{
    [Test]
    public async Task Test()
    {
        var serviceProvider = new ServiceCollection()
            .AddActionCacheRedis(options => options.Configuration = "127.0.0.1:6379")
            .BuildServiceProvider();

        var cacheFactory = serviceProvider.GetRequiredService<IActionCacheFactory>();
        var cache = cacheFactory.Create("Test")!;

        await cache.SetAsync("Foo", "Bar");
        await cache.SetAsync("Biz", "Baz");
        await cache.SetAsync("Coz", "Doz");
        await cache.RemoveAsync();

        string?[] result = [
            await cache.GetAsync<string>("Foo"),
            await cache.GetAsync<string>("Biz"),
            await cache.GetAsync<string>("Coz")
        ];

        Assert.That(result, Is.All.Null);
    }
}