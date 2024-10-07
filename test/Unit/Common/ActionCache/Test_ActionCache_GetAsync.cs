using ActionCache;
using Microsoft.Extensions.DependencyInjection;
using Unit.TestUtiltiies.Data;

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

    [Test]
    [TestCaseSource(typeof(TestData), nameof(TestData.GetServiceProviders))]
    public async Task Test_NullableInt_ReturnsNull(IServiceProvider serviceProvider)
    {
        var cacheFactory = serviceProvider.GetRequiredService<IActionCacheFactory>();
        var cache = cacheFactory.Create("Test")!;
        var result = await cache.GetAsync<int?>("Foo_Not_Present");
        Assert.That(result, Is.EqualTo(null));
    }
}