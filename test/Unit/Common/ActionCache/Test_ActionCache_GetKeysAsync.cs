using ActionCache;
using Microsoft.Extensions.DependencyInjection;
using Unit.TestUtiltiies.Data;

namespace Unit.Common;

[TestFixture]
public class Test_ActionCache_GetKeysAsync
{
    [Test]
    [TestCaseSource(typeof(TestData), nameof(TestData.GetServiceProviders))]
    public async Task Test(IServiceProvider serviceProvider)
    {
        var cacheFactory = serviceProvider.GetRequiredService<IActionCacheFactory>();
        var cache = cacheFactory.Create("Test")!;
        await cache.RemoveAsync();

        await cache.SetAsync("Foo", "Bar");
        await cache.SetAsync("Biz", "Baz");

        var result = await cache.GetKeysAsync();
        Assert.That(result.Count(), Is.EqualTo(2));
    }
}