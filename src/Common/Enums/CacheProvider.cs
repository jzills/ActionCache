namespace ActionCache;

/// <summary>
/// Defines the types of caches supported.
/// </summary>
public enum CacheType
{
    /// <summary>
    /// In-memory caching.
    /// </summary>
    Memory,

    /// <summary>
    /// Redis-based caching.
    /// </summary>
    Redis,

    /// <summary>
    /// SQL Server-based caching.
    /// </summary>
    SqlServer
}