using ActionCache;
using Microsoft.Extensions.DependencyInjection;
using Unit.TestUtiltiies.Data;

namespace Unit.Common;

[TestFixture]
public class Test_ActionCache_Expiration
{
    [Test]
    [TestCaseSource(typeof(TestData), nameof(TestData.GetServiceProviders))]
    public async Task Test_GetAsync_Expires(IServiceProvider serviceProvider)
    {
        var cacheFactory = serviceProvider.GetRequiredService<IActionCacheFactory>();
        var cache = cacheFactory.Create("Test_Redis_Expiration", TimeSpan.FromSeconds(5));
    
        await cache.SetAsync("Key_Expiration_1", "Value_1");
        var result = await cache.GetAsync<string?>("Key_Expiration_1");
        var keys = await cache.GetKeysAsync();

        Assert.That(result, Is.EqualTo("Value_1"));
        Assert.That(keys.Count(), Is.EqualTo(1));

        Thread.Sleep(5000);

        result = await cache.GetAsync<string?>("Key_Expiration_1");
        keys = await cache.GetKeysAsync();
        
        Assert.That(result, Is.Null);
        Assert.That(keys.Count(), Is.EqualTo(0));
    }

    [Test]
    [TestCaseSource(typeof(TestData), nameof(TestData.GetServiceProviders))]
    public async Task Test_GetKeys_Expires(IServiceProvider serviceProvider)
    {
        var cacheFactory = serviceProvider.GetRequiredService<IActionCacheFactory>();
        var cache = cacheFactory.Create("Test_Redis_Expiration", TimeSpan.FromSeconds(5));
    
        await cache.SetAsync("Key_Expiration_1", "Value_1");
        var result = await cache.GetAsync<string?>("Key_Expiration_1");
        var keys = await cache.GetKeysAsync();

        Assert.That(result, Is.EqualTo("Value_1"));
        Assert.That(keys.Count(), Is.EqualTo(1));

        Thread.Sleep(5000);

        keys = await cache.GetKeysAsync();
        result = await cache.GetAsync<string?>("Key_Expiration_1");
        
        Assert.That(result, Is.Null);
        Assert.That(keys.Count(), Is.EqualTo(0));
    }
}