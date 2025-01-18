using ActionCache;
using Microsoft.Extensions.DependencyInjection;
using Unit.TestUtiltiies.Data;

namespace Unit.SqlServer;

[TestFixture]
public class Test_SqlServerActionCache_SetAsync
{
    [Test]
    [TestCaseSource(typeof(TestData), nameof(TestData.GetSqlServerServiceProvider))]
    public async Task Test(IServiceProvider serviceProvider)
    {
        var cacheFactory = serviceProvider.GetRequiredService<IActionCacheFactory>();
        var cache = cacheFactory.Create(nameof(Test_SqlServerActionCache_SetAsync));
        await cache.SetAsync("Bar", "Biz");

        var result = await cache.GetAsync<string>("Bar");
        Assert.That(result, Is.EqualTo("Biz"));
    }
}