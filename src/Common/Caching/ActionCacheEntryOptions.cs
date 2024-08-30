namespace ActionCache.Common;

/// <summary>
/// Represents cache entry options for an action cache.
/// </summary>
public class ActionCacheEntryOptions
{
    /// <summary>
    /// Gets or sets the absolute expiration time as a relative duration.
    /// </summary>
    /// <value>The duration from now to expire the cache.</value>
    public TimeSpan? AbsoluteExpiration { get; set; }

    /// <summary>
    /// Gets or sets the sliding expiration time.
    /// </summary>
    /// <value>The sliding duration, which resets whenever the cache entry is accessed.</value>
    public TimeSpan? SlidingExpiration { get; set; }
}