using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.SqlServer;
using Microsoft.Extensions.DependencyInjection;
using Unit.TestUtiltiies.Data;

namespace Unit.SqlServer;

[TestFixture]
public class Test_SqlServerActionCache_Type
{
    [Test]
    [TestCaseSource(typeof(TestData), nameof(TestData.GetSqlServerServiceProvider))]
    public void Test(IServiceProvider serviceProvider)
    {
        var cache = serviceProvider.GetRequiredService<IDistributedCache>();
        Assert.That(cache, Is.TypeOf<SqlServerCache>());
    }
}