using ActionCache;
using ActionCache.Memory.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Unit.Memory;

[TestFixture]
public class Test_MemoryActionCache_RemoveAsync
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
        await cache.RemoveAsync("Foo");

        var result = await cache.GetAsync<string>("Foo");
        Assert.That(result, Is.Null);
    }
}