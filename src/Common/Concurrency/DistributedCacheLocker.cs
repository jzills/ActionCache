using Microsoft.Extensions.Caching.Distributed;

namespace ActionCache.Common.Concurrency;

/// <summary>
/// Provides a mechanism for acquiring and releasing locks in a distributed cache system.
/// </summary>
public class DistributedCacheLocker : CacheLockerBase<DistributedCacheLock>
{
    /// <summary>
    /// The distributed cache instance used to store lock information.
    /// </summary>
    protected readonly IDistributedCache Cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="DistributedCacheLocker"/> class.
    /// </summary>
    /// <param name="cache">The distributed cache instance to be used for managing locks.</param>
    public DistributedCacheLocker(IDistributedCache cache, TimeSpan lockDuration, TimeSpan lockTimeout) 
        : base(lockDuration, lockTimeout)
    {
        Cache = cache;
    }

    /// <inheritdoc/>
    public override async Task<DistributedCacheLock> TryAcquireLockAsync(string resource)
    {
        var cacheLock = new DistributedCacheLock(resource, LockDuration, LockTimeout);
        var existingCacheLock = await Cache.GetStringAsync(cacheLock.Key);
        if (existingCacheLock == null)
        {
            await Cache.SetStringAsync(cacheLock.Key, cacheLock.Value, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheLock.Duration
            });

            // Perform one more get to ensure this lock value is the one
            // in the cache and we didn't hit a race condition. Even so, this does
            // not guarantee atomicity but it's better than nothing.
            var existingCacheLockValue = await Cache.GetStringAsync(cacheLock.Key);

            cacheLock.IsAcquired = existingCacheLockValue == cacheLock.Value;
        }

        return cacheLock;
    }

    /// <inheritdoc/>
    public override async Task ReleaseLockAsync(DistributedCacheLock cacheLock)
    {
        var currentValue = await Cache.GetStringAsync(cacheLock.Key);
        if (currentValue == cacheLock.Value)
        {
            await Cache.RemoveAsync(cacheLock.Key);
        }
    }

    /// <inheritdoc/>
    public override async Task<DistributedCacheLock> WaitForLockAsync(string resource)
    {
        var cacheLock = new DistributedCacheLock(resource, LockDuration, LockTimeout);
        while (cacheLock.ShouldTryAcquire())
        {
            var acquiredLock = await TryAcquireLockAsync(cacheLock.Resource);
            if (acquiredLock.IsAcquired)
            {
                cacheLock = acquiredLock;
                break;
            }

            await Task.Delay(100); 
        }

        return cacheLock;
    }
}