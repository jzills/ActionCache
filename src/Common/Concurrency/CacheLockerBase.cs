namespace ActionCache.Common.Concurrency;

/// <summary>
/// Base class for cache lockers, responsible for acquiring and releasing locks on cache resources.
/// </summary>
/// <typeparam name="TLock">The type of the cache lock used for this locker.</typeparam>
public abstract class CacheLockerBase<TLock> : ICacheLocker<TLock> where TLock : CacheLock
{
    /// <summary>
    /// The duration for which the lock will be held.
    /// </summary>
    protected readonly TimeSpan LockDuration;

    /// <summary>
    /// The timeout period for trying to acquire the lock.
    /// </summary>
    protected readonly TimeSpan LockTimeout;

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheLockerBase{T}"/> class with the specified lock duration and timeout.
    /// </summary>
    /// <param name="lockDuration">The duration for which the lock will be held.</param>
    /// <param name="lockTimeout">The timeout period for trying to acquire the lock.</param>
    public CacheLockerBase(TimeSpan lockDuration, TimeSpan lockTimeout)
    {
        LockDuration = lockDuration;
        LockTimeout = lockTimeout;
    }

    /// <summary>
    /// Releases the specified cache lock.
    /// </summary>
    /// <param name="cacheLock">The cache lock to release.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public abstract Task ReleaseLockAsync(TLock cacheLock);

    /// <summary>
    /// Attempts to acquire a lock for the specified resource.
    /// </summary>
    /// <param name="resource">The resource to acquire the lock for.</param>
    /// <returns>A task that represents the asynchronous operation, with the acquired cache lock.</returns>
    public abstract Task<TLock> TryAcquireLockAsync(string resource);

    /// <summary>
    /// Waits for the lock on the specified resource to be acquired and returns the lock.
    /// </summary>
    /// <param name="resource">The resource to acquire the lock for.</param>
    /// <returns>A task that represents the asynchronous operation, with the acquired cache lock.</returns>
    public abstract Task<TLock> WaitForLockAsync(string resource);

    /// <summary>
    /// Asynchronously waits for a lock to be acquired, then executes an action if the lock is acquired.
    /// </summary>
    /// <param name="resource">The resource for which the lock is requested.</param>
    /// <param name="thenFunc">The action to be executed after the lock is acquired.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async Task WaitForLockThenAsync(string resource, Action thenFunc)
    {
        await WaitForLockThenAsync(resource, () => {
            thenFunc();
            return Task.CompletedTask;
        });
        // var cacheLock = await WaitForLockAsync(resource);
        // if (cacheLock.IsAcquired)
        // {
        //     try
        //     {
        //         thenFunc();
        //     }
        //     finally
        //     {
        //         await ReleaseLockAsync(cacheLock);
        //     }
        // }
        // else
        // {
        //     // Handle lock acquisition failure (optional custom handling logic can be added here)
        // }
    }

    /// <summary>
    /// Asynchronously waits for a lock to be acquired, then executes a function that returns a result if the lock is acquired.
    /// </summary>
    /// <typeparam name="TResult">The type of the result returned by the function.</typeparam>
    /// <param name="resource">The resource for which the lock is requested.</param>
    /// <param name="resultAccessor">The function that will return a result once the lock is acquired.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result of the function or default value if lock is not acquired.</returns>
    public virtual async Task<TResult?> WaitForLockThenAsync<TResult>(string resource, Func<TResult> resultAccessor)
    {
        return await WaitForLockThenAsync(resource, () => Task.FromResult(resultAccessor()));

        // TResult? result = default;

        // var cacheLock = await WaitForLockAsync(resource);
        // if (cacheLock.IsAcquired)
        // {
        //     try
        //     {
        //         result = resultAccessor();
        //     }
        //     finally
        //     {
        //         await ReleaseLockAsync(cacheLock);
        //     }
        // }
        // else
        // {
        //     // Handle lock acquisition failure (optional custom handling logic can be added here)
        // }

        // return result;
    }

    /// <summary>
    /// Waits for the lock to be acquired on the specified resource, then executes the provided action.
    /// </summary>
    /// <param name="resource">The resource to acquire the lock for.</param>
    /// <param name="thenFunc">The action to execute once the lock is acquired.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// The action will be executed after the lock is acquired and released once the action completes.
    /// If the lock acquisition fails, no action is executed.
    /// </remarks>
    public virtual async Task WaitForLockThenAsync(string resource, Func<Task> thenFunc)
    {
        var cacheLock = await WaitForLockAsync(resource);
        if (cacheLock.IsAcquired)
        {
            try
            {
                await thenFunc();
            }
            finally
            {
                await ReleaseLockAsync(cacheLock);
            }
        }
        else
        {
            // Handle lock acquisition failure
        }
    }

    /// <summary>
    /// Waits for the lock to be acquired on the specified resource, then executes the provided function and returns the result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result produced by the function.</typeparam>
    /// <param name="resource">The resource to acquire the lock for.</param>
    /// <param name="resultAccessor">The function that will be executed once the lock is acquired, which returns a result.</param>
    /// <returns>A task representing the asynchronous operation, with the result of the function.</returns>
    /// <remarks>
    /// The result will be returned after the lock is acquired and released once the function completes.
    /// If the lock acquisition fails, the result will be null.
    /// </remarks>
    public virtual async Task<TResult?> WaitForLockThenAsync<TResult>(string resource, Func<Task<TResult>> resultAccessor)
    {
        TResult? result = default;

        var cacheLock = await WaitForLockAsync(resource);
        if (cacheLock.IsAcquired)
        {
            try
            {
                result = await resultAccessor();
            }
            finally
            {
                await ReleaseLockAsync(cacheLock);
            }
        }
        else
        {
            // Handle lock acquisition failure
        }

        return result;
    }
}