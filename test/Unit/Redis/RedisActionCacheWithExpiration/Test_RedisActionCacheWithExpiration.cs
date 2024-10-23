using ActionCache.Common.Extensions;
using ActionCache.Redis;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Unit.TestUtiltiies.Data;

namespace Unit.Redis;

[TestFixture]
public class Test_RedisActionCacheWithExpiration
{
    [Test]
    [TestCaseSource(typeof(TestData), nameof(TestData.GetRedisCacheServiceProvider))]
    public async Task Test(IServiceProvider serviceProvider)
    {
        var database = serviceProvider.GetRequiredService<IConnectionMultiplexer>().GetDatabase();
        var cache = new RedisActionCacheWithExpiration("Test_Redis_Expiration", 
            database, 
            new ActionCache.Common.ActionCacheEntryOptions { AbsoluteExpiration = TimeSpan.FromSeconds(5) }, 
            new ActionCache.Common.Caching.ActionCacheRefreshProvider(null)
        );

        await cache.SetAsync("Key_Expiration_1", "Value_1");
        var keys = await cache.GetKeysAsync();
        Assert.That(keys.Count(), Is.EqualTo(1));

        Thread.Sleep(5000);

        var result = await cache.GetAsync<string?>("Key_Expiration_1");
        keys = await cache.GetKeysAsync();
        
        Assert.That(result, Is.Null);
        Assert.That(keys.Count(), Is.EqualTo(0));
    }
}