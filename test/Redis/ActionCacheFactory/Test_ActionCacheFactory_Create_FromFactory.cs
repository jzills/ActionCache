using ActionCache;
using ActionCache.Redis;
using ActionCache.Redis.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Unit.Redis;

[TestFixture]
public class Test_ActionCacheFactory_Create_FromFactory
{
    [Test]
    public void Test()
    {
        var serviceProvider = new ServiceCollection()
            .AddActionCacheRedis(options => options.Configuration = "127.0.0.1:6379")
            .BuildServiceProvider();

        var cacheFactory = serviceProvider.GetRequiredService<IActionCacheFactory>();
        var cache = cacheFactory.Create("MyNamespace");

        Assert.That(cache, Is.Not.Null);
        Assert.That(cache, Is.TypeOf<RedisActionCache>());
    }
}