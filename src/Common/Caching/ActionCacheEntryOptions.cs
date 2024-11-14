namespace ActionCache.Common;

/// <summary>
/// Represents cache entry options for an action cache, allowing configuration of absolute and sliding expirations.
/// </summary>
public class ActionCacheEntryOptions
{
    /// <summary>
    /// Represents a value indicating no expiration for the cache entry.
    /// </summary>
    internal const long NoExpiration = 0;

    /// <summary>
    /// Gets or sets the absolute expiration time as a duration relative to now.
    /// </summary>
    /// <value>The time from now at which the cache entry expires.</value>
    public TimeSpan? AbsoluteExpiration { get; set; }

    /// <summary>
    /// Gets or sets the sliding expiration time, which resets whenever the cache entry is accessed.
    /// </summary>
    /// <value>The sliding expiration duration.</value>
    public TimeSpan? SlidingExpiration { get; set; }

    /// <summary>
    /// Calculates the absolute expiration date and time based on <see cref="AbsoluteExpiration"/>, relative to the current UTC time.
    /// </summary>
    /// <returns>A <see cref="DateTimeOffset"/> representing the absolute expiration date and time, or <c>null</c> if <see cref="AbsoluteExpiration"/> is not set.</returns>
    public DateTimeOffset? GetAbsoluteExpirationFromUtcNow() =>
        AbsoluteExpiration.HasValue ?
            DateTimeOffset.UtcNow.Add(AbsoluteExpiration.Value) :
            null;

    /// <summary>
    /// Retrieves the absolute expiration time-to-live (TTL) in milliseconds.
    /// </summary>
    /// <returns>A <c>long</c> representing the TTL in milliseconds, or <see cref="NoExpiration"/> if <see cref="AbsoluteExpiration"/> is not set.</returns>
    public long GetAbsoluteExpirationAsTTLInMilliseconds() =>
        AbsoluteExpiration.HasValue ?
            (long)AbsoluteExpiration.Value.TotalMilliseconds :
            NoExpiration;

    /// <summary>
    /// Calculates the absolute expiration time from the current UTC time, in milliseconds since the Unix epoch.
    /// </summary>
    /// <returns>A <c>long</c> representing the absolute expiration in milliseconds from the Unix epoch, or <see cref="NoExpiration"/> if <see cref="AbsoluteExpiration"/> is not set.</returns>
    public long GetAbsoluteExpirationFromUtcNowInMilliseconds() =>
        GetAbsoluteExpirationFromUtcNow()?.ToUnixTimeMilliseconds() ?? NoExpiration;

    /// <summary>
    /// Retrieves the sliding expiration time in milliseconds.
    /// </summary>
    /// <returns>A <c>long</c> representing the sliding expiration in milliseconds, or <see cref="NoExpiration"/> if <see cref="SlidingExpiration"/> is not set.</returns>
    public long GetSlidingExpirationInMilliseconds() =>
        SlidingExpiration.HasValue ?
            (long)SlidingExpiration.Value.TotalMilliseconds :
            NoExpiration;
}