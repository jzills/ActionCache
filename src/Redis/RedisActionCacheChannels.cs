namespace ActionCache.Redis;

/// <summary>
/// Defines the Redis channels used by the RedisActionCache.
/// </summary>
public class RedisActionCacheChannels
{
    /// <summary>
    /// The main Redis channel for the RedisActionCache.
    /// </summary>
    public const string Main = $"{nameof(ActionCache)}:{nameof(Main)}";
}