using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace ActionCache.Common;

/// <summary>
/// Provides configuration options for action caching.
/// </summary>
public class ActionCacheOptions
{
    /// <summary>
    /// Gets the default entry options for the cache.
    /// </summary>
    public readonly ActionCacheEntryOptions EntryOptions = new();

    /// <summary>
    /// Gets or sets a delegate to configure options for <see cref="MemoryCacheOptions"/>.
    /// </summary>
    public Action<MemoryCacheOptions>? ConfigureMemoryCacheOptions { get; set; }

    /// <summary>
    /// Gets or sets a delegate to configure options for <see cref="RedisCacheOptions"/>.
    /// </summary>
    public Action<RedisCacheOptions>? ConfigureRedisCacheOptions { get; set; }

    /// <summary>
    /// Gets the dictionary that indicates whether a specific cache type is enabled.
    /// </summary>
    public readonly IDictionary<CacheType, bool> EnabledCaches = 
        new Dictionary<CacheType, bool>
        {
            [CacheType.Redis]     = false,
            [CacheType.Memory]    = false,
            [CacheType.SqlServer] = false
        };
}