using ActionCache;
using Microsoft.Extensions.DependencyInjection;
using Unit.TestUtiltiies.Data;

namespace Unit.Common;

[TestFixture]
public class Test_ActionCache_RemoveAsync_Namespace
{
    [Test]
    [TestCaseSource(typeof(TestData), nameof(TestData.GetAzureCosmosServiceProvider))]
    public async Task Test(IServiceProvider serviceProvider)
    {
        var cacheFactory = serviceProvider.GetRequiredService<IActionCacheFactory>();
        var cache = cacheFactory.Create(nameof(Test_ActionCache_RemoveAsync_Namespace))!;

        // await cache.SetAsync("Foo", "Bar");
        // await cache.SetAsync("Biz", "Baz");
        // await cache.SetAsync("Coz", "Doz");
        await cache.RemoveAsync();

        string?[] result = [
            await cache.GetAsync<string>("Foo"),
            await cache.GetAsync<string>("Biz"),
            await cache.GetAsync<string>("Coz")
        ];

        Assert.That(result, Is.All.Null);
    }
}