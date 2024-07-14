using ActionCache;
using ActionCache.Memory;
using ActionCache.Memory.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Unit.Memory;

[TestFixture]
public class Test_ActionCacheFactory_Create_FromFactory
{
    [Test]
    public void Test()
    {
        var serviceProvider = new ServiceCollection()
            .AddMemoryCache()
            .AddActionCacheMemory(options => options.SizeLimit = int.MaxValue)
            .BuildServiceProvider();

        var cacheFactory = serviceProvider.GetRequiredService<IActionCacheFactory>();
        var cache = cacheFactory.Create("MyNamespace");

        Assert.That(cache, Is.Not.Null);
        Assert.That(cache, Is.InstanceOf<MemoryActionCache>());
    }
}