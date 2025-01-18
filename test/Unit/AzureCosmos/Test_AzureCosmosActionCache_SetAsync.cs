using ActionCache;
using Microsoft.Extensions.DependencyInjection;
using Unit.TestUtiltiies.Data;

namespace Unit.AzureCosmos;

[TestFixture]
public class Test_AzureCosmosActionCache_SetAsync
{
    [Test]
    [TestCaseSource(typeof(TestData), nameof(TestData.GetAzureCosmosServiceProvider))]
    public async Task Test(IServiceProvider serviceProvider)
    {
        var cacheFactory = serviceProvider.GetRequiredService<IActionCacheFactory>();
        var cache = cacheFactory.Create(nameof(Test_AzureCosmosActionCache_SetAsync));
        await cache.SetAsync("Bar", "Biz");

        var result = await cache.GetAsync<string>("Bar");
        Assert.That(result, Is.EqualTo("Biz"));

        await cache.RemoveAsync("Bar");
        result = await cache.GetAsync<string>("Bar");

        Assert.That(result, Is.EqualTo(null));
    }
}