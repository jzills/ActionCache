using ActionCache.Common.Concurrency;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Unit.TestUtiltiies.Data;

namespace Unit.SqlServer;

[TestFixture]
public class Test_DistributedCacheLocker
{
    [Test]
    [TestCaseSource(typeof(TestData), nameof(TestData.GetSqlServerServiceProvider))]
    public async Task Test(IServiceProvider serviceProvider)
    {
        var cache = serviceProvider.GetRequiredService<IDistributedCache>();
        cache.SetString("Lock:Namespace", string.Empty);

        var cacheLocker = new DistributedCacheLocker(cache, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5));
        var cacheLock = await cacheLocker.WaitForLockAsync("Namespace");

        Assert.That(cacheLock.IsAcquired, Is.False);
    }

    [Test]
    public async Task Test_DistributedCacheLock_ShouldAcquire()
    {
        var cacheLock = new DistributedCacheLock("Namespace");
        Assert.That(cacheLock.ShouldTryAcquire(), Is.True);
    }
}