using ActionCache;
using ActionCache.Redis.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Unit.Redis;

[TestFixture]
public class Test_RedisActionCache_RemoveAsync
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
        await cache.RemoveAsync("Foo");

        var result = await cache.GetAsync<string>("Foo");
        Assert.That(result, Is.Null);
    }
}