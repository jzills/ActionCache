namespace ActionCache.Common;

/// <summary>
/// Represents cache entry options for an action cache.
/// </summary>
public class ActionCacheEntryOptions
{
    internal const long NoExpiration = 0;

    /// <summary>
    /// Gets or sets the absolute expiration time as a time relative to now.
    /// </summary>
    /// <value>The time from now to expire the cache entry.</value>
    public TimeSpan? AbsoluteExpiration { get; set; }

    /// <summary>
    /// Gets or sets the sliding expiration time.
    /// </summary>
    /// <value>The sliding duration, which resets whenever the cache entry is accessed.</value>
    public TimeSpan? SlidingExpiration { get; set; }

    public DateTimeOffset? GetAbsoluteExpirationFromUtcNow()
    {
        if (AbsoluteExpiration.HasValue)
        {
            return DateTimeOffset.UtcNow.Add(AbsoluteExpiration.Value);
        }
        else
        {
            return null;
        }
    }

    public long GetAbsoluteExpirationFromUtcNowInMilliseconds()
    {
        var expirationDateTime = GetAbsoluteExpirationFromUtcNow();
        return expirationDateTime?.ToUnixTimeMilliseconds() ?? NoExpiration;
    }

    public long GetSlidingExpirationInMilliseconds()
    {
        if (SlidingExpiration.HasValue)
        {
            return (long)SlidingExpiration.Value.TotalMilliseconds;
        }
        else
        {
            return NoExpiration;
        }
    }
}