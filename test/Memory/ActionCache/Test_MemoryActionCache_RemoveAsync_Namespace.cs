using ActionCache;
using ActionCache.Memory.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Unit.Memory;

[TestFixture]
public class Test_MemoryActionCache_RemoveAsync_Namespace
{
    [Test]
    public async Task Test()
    {
        var serviceProvider = new ServiceCollection()
            .AddMemoryCache()
            .AddActionCacheMemory(options => options.SizeLimit = int.MaxValue)
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