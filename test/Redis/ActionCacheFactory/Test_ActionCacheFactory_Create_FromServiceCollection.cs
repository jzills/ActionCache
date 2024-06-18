using ActionCache;
using ActionCache.Redis;
using ActionCache.Redis.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Unit.Redis;

[TestFixture]
public class Test_ActionCacheFactory_Create_FromServiceCollection
{
    [Test]
    public void Test()
    {
        var serviceProvider = new ServiceCollection()
            .AddActionCacheRedis(options => options.Configuration = "127.0.0.1:6379")
            .BuildServiceProvider();

        var cacheFactory = serviceProvider.GetRequiredService<IActionCacheFactory>();

        Assert.That(cacheFactory, Is.Not.Null);
        Assert.That(cacheFactory, Is.InstanceOf<RedisActionCacheFactory>());
    }
}