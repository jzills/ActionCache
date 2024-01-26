using Microsoft.Extensions.DependencyInjection;
using ActionCache.Extensions;
using ActionCache;

namespace Unit;

public class ServiceCollection_AddActionCache_InMemory_AddThenRemove
{
    [Test]
    public async Task Test()
    {
        var serviceProvider = new ServiceCollection()
            .AddMemoryCache()
            .AddActionCache(options => options.Namespace = "MyNamespace")
            .BuildServiceProvider();

        var cache = serviceProvider.GetRequiredService<IActionCache>();
        
        for (int i = 0; i < 100; i++)
        {
            await cache.SetAsync($"Key{i}", $"Value{i}");
        }

        for (int i = 0; i < 100; i++)
        {
            var value = await cache.GetAsync<string>($"Key{i}");
            Assert.AreEqual(value, $"Value{i}");
        }

        await cache.RemoveAsync();

        for (int i = 0; i < 100; i++)
        {
            var value = await cache.GetAsync<string>($"Key{i}");
            Assert.AreEqual(value, null);
        }
    }
}