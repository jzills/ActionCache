using System.Collections.Concurrent;

namespace ActionCache.Common.Concurrency;

/// <summary>
/// Provides a locking mechanism using <see cref="SemaphoreSlim"/> for resource synchronization in a distributed cache system.
/// </summary>
public class SemaphoreSlimLocker : CacheLockerBase<SemaphoreSlimLock>
{
    /// <summary>
    /// A thread-safe dictionary to manage semaphores for each resource.
    /// </summary>
    protected readonly ConcurrentDictionary<string, SemaphoreSlim> Semaphores = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="SemaphoreSlimLocker"/> class with the specified lock duration and timeout.
    /// </summary>
    /// <param name="lockDuration">The duration for which the lock should be held before it expires.</param>
    /// <param name="lockTimeout">The maximum time to wait for acquiring the lock.</param>
    public SemaphoreSlimLocker(TimeSpan lockDuration, TimeSpan lockTimeout) 
        : base(lockDuration, lockTimeout)
    {
    }

    /// <inheritdoc/>
    public override async Task<SemaphoreSlimLock> TryAcquireLockAsync(string resource)
    {
        var cacheLock = new SemaphoreSlimLock(resource, LockDuration, LockTimeout);
        var semaphore = Semaphores.GetOrAdd(resource, _ => new SemaphoreSlim(1, 1));

        cacheLock.IsAcquired = await semaphore.WaitAsync(cacheLock.Timeout);
        return cacheLock;
    }

    /// <inheritdoc/>
    public override Task ReleaseLockAsync(SemaphoreSlimLock cacheLock)
    {
        if (cacheLock.IsAcquired)
        {
            if (Semaphores.TryGetValue(cacheLock.Resource, out var semaphore))
            {
                semaphore.Release();
            }
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public override Task<SemaphoreSlimLock> WaitForLockAsync(string resource) => TryAcquireLockAsync(resource);
}