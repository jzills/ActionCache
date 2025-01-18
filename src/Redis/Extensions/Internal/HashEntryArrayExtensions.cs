using StackExchange.Redis;

namespace ActionCache.Redis.Extensions.Internal;

internal static class HashEntryArrayExtensions
{
    internal static RedisValue GetRedisValue(this HashEntry[] entries) =>
        entries.GetRedisValue(RedisHashEntry.Value);

    internal static long GetAbsoluteExpiration(this HashEntry[] entries) =>
        (long)entries.GetRedisValue(RedisHashEntry.AbsoluteExpiration);
    
    internal static long GetSlidingExpiration(this HashEntry[] entries) => 
        (long)entries.GetRedisValue(RedisHashEntry.SlidingExpiration);

    private static RedisValue GetRedisValue(this HashEntry[] entries, string name) => 
        entries.First(entry => entry.Name == name).Value;
}