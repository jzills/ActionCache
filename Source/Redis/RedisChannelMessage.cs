using ActionCache.Utilities;

namespace ActionCache.Redis;

public record class RedisChannelMessage(
    string Namespace, 
    string? Key = null,
    object? Value = null,
    MessageTypes Type = MessageTypes.Set
);