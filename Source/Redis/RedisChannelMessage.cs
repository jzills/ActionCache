namespace ActionCache.Redis;

public record class RedisChannelMessage(
    string Namespace, 
    string? Key = null,
    MessageTypes Type = MessageTypes.Set
);

public record class RedisChannelMessage<TValue> : RedisChannelMessage
{
    public readonly TValue? Value;

    public RedisChannelMessage(
        string Namespace, 
        string Key,
        TValue? value
    ) : base(Namespace, Key, MessageTypes.Set)
    {
        Value = value;
    }
}