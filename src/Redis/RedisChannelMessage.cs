using ActionCache.Utilities;

namespace ActionCache.Redis;

public record class RedisChannelMessage(
    string Namespace, 
    string? Key = null,
    object? Value = null,
    MessageType Type = MessageType.Set
);