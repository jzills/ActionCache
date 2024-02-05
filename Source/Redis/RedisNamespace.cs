using StackExchange.Redis;
using ActionCache.Utilities;

namespace ActionCache.Redis;

public record class RedisNamespace(string Value) : Namespace(Value)
{
    public static implicit operator RedisKey(RedisNamespace @this) => new RedisKey(@this);
    public static implicit operator RedisNamespace(string @namespace) => new RedisNamespace(@namespace);
}