using ActionCache;
using Microsoft.Extensions.DependencyInjection;
using Unit.TestUtiltiies.Data;

namespace Unit.Common;

[TestFixture]
public class Test_ActionCache_RemoveAsync
{
    [Test]
    [TestCaseSource(typeof(TestData), nameof(TestData.GetServiceProviders))]
    public async Task Test(IServiceProvider serviceProvider)
    {
        var cacheFactory = serviceProvider.GetRequiredService<IActionCacheFactory>();
        var cache = cacheFactory.Create(nameof(Test_ActionCache_RemoveAsync))!;

        await cache.SetAsync("Foo", "Bar");
        await cache.RemoveAsync("Foo");

        var result = await cache.GetAsync<string>("Foo");
        Assert.That(result, Is.Null);
    }
}