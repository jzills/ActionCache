using ActionCache;
using ActionCache.Memory;
using ActionCache.Memory.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Unit.Memory;

[TestFixture]
public class Test_ActionCacheFactory_Create_FromServiceCollection
{
    [Test]
    public void Test()
    {
        var serviceProvider = new ServiceCollection()
            .AddMemoryCache()
            .AddActionCacheMemory(options => options.SizeLimit = int.MaxValue)
            .BuildServiceProvider();

        var cacheFactory = serviceProvider.GetRequiredService<IActionCacheFactory>();

        Assert.That(cacheFactory, Is.Not.Null);
        Assert.That(cacheFactory, Is.InstanceOf<MemoryActionCacheFactory>());
    }
}