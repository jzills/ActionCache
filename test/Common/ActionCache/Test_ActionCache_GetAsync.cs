using ActionCache;
using Microsoft.Extensions.DependencyInjection;
using Unit.Common.Data;

namespace Unit.Common;

[TestFixture]
public class Test_ActionCache_GetAsync
{
    [Test]
    [TestCaseSource(typeof(TestData), nameof(TestData.GetServiceProviders))]
    public async Task Test(IServiceProvider serviceProvider)
    {
        var cacheFactory = serviceProvider.GetRequiredService<IActionCacheFactory>();
        var cache = cacheFactory.Create("Test")!;
        await cache.SetAsync("Foo", "Bar");

        var result = await cache.GetAsync<string>("Foo");
        Assert.That(result, Is.EqualTo("Bar"));
    }
}