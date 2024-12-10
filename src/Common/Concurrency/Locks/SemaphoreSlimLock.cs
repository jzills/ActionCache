namespace ActionCache.Common.Concurrency;

/// <summary>
/// Represents the lock state for a specific resource using SemaphoreSlim.
/// </summary>
public class SemaphoreSlimLock : CacheLock
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SemaphoreSlimLock"/> class with the specified resource, duration, and timeout.
    /// </summary>
    /// <param name="resource">The unique identifier of the resource to lock.</param>
    /// <param name="duration">The duration for which the lock should be held.</param>
    /// <param name="timeout">The maximum time allowed for attempting to acquire the lock.</param>
    public SemaphoreSlimLock(string resource, TimeSpan duration, TimeSpan timeout) : base(resource)
    {
        Duration = duration;
        Timeout = timeout;
        IsAcquired = false;
    }
}