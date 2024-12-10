namespace ActionCache.Common.Concurrency;

/// <summary>
/// Represents a distributed lock mechanism that can be used with a distributed cache.
/// </summary>
public class DistributedCacheLock : CacheLock
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DistributedCacheLock"/> class with a resource, duration, and timeout.
    /// </summary>
    /// <param name="resource">The unique resource identifier to lock.</param>
    /// <param name="duration">The duration for which the lock is held.</param>
    /// <param name="timeout">The timeout duration for attempting to acquire the lock.</param>
    public DistributedCacheLock(string resource, TimeSpan duration, TimeSpan timeout) : base(resource)
    {
        Duration = duration;
        Timeout = timeout;
    }

    /// <summary>
    /// Gets the cache key used for storing the lock in the distributed cache.
    /// </summary>
    public string Key => $"Lock:{Resource}";

    /// <summary>
    /// Gets or sets the unique value associated with the lock, typically a GUID.
    /// </summary>
    public string Value { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Determines whether the lock acquisition should be attempted.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the lock is not already acquired and the timeout has not been exceeded; otherwise, <c>false</c>.
    /// </returns>
    public bool ShouldTryAcquire() => !(IsAcquired || HasExceededTimeout());

    /// <summary>
    /// Checks whether the timeout period for acquiring the lock has been exceeded.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the current time is greater than or equal to the requested time plus the timeout duration; otherwise, <c>false</c>.
    /// </returns>
    public bool HasExceededTimeout() => DateTime.UtcNow >= DateRequested.Add(Timeout);
}