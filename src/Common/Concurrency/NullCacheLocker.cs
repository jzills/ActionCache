using ActionCache.Common.Concurrency.Locks;

namespace ActionCache.Common.Concurrency;

/// <summary>
/// Represents a no-operation cache locker that does not perform any actual locking mechanism.
/// It serves as a fallback implementation where a locking system is required but not enforced.
/// </summary>
public class NullCacheLocker : CacheLockerBase<NullCacheLock>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NullCacheLocker"/> class.
    /// </summary>
    /// <param name="lockDuration">The duration for which the lock is considered valid.</param>
    /// <param name="lockTimeout">The maximum time to wait for acquiring a lock.</param>
    public NullCacheLocker(TimeSpan lockDuration, TimeSpan lockTimeout) 
        : base(lockDuration, lockTimeout)
    {
    }

    /// <summary>
    /// Releases the specified lock asynchronously. Since this is a no-operation implementation, 
    /// the method completes immediately.
    /// </summary>
    /// <param name="cacheLock">The lock to be released.</param>
    /// <returns>A completed task.</returns>
    public override Task ReleaseLockAsync(NullCacheLock cacheLock) => Task.CompletedTask;

    /// <summary>
    /// Attempts to acquire a lock for the specified resource asynchronously.
    /// Since this is a no-operation implementation, it always returns a new lock immediately.
    /// </summary>
    /// <param name="resource">The resource to lock.</param>
    /// <returns>A completed task containing the acquired lock.</returns>
    public override Task<NullCacheLock> TryAcquireLockAsync(string resource) => 
        Task.FromResult(new NullCacheLock(resource));

    /// <summary>
    /// Waits for a lock on the specified resource asynchronously.
    /// Since this is a no-operation implementation, it immediately returns a new lock.
    /// </summary>
    /// <param name="resource">The resource to lock.</param>
    /// <returns>A completed task containing the acquired lock.</returns>
    public override Task<NullCacheLock> WaitForLockAsync(string resource) => TryAcquireLockAsync(resource);
}