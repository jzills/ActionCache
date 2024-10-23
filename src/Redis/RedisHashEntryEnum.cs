namespace ActionCache.Redis;

public static class RedisHashEntryEnum
{
    public const string Value = "VALUE";
    public const string AbsoluteExpiration = "ABSOLUTE_EXPIRATION";
    public const string SlidingExpiration = "SLIDING_EXPIRATION";
}