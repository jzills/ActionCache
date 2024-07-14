using ActionCache.Utilities;

namespace ActionCache.Redis;

/// <summary>
/// Represents a message to be published to a Redis channel.
/// </summary>
public record class RedisChannelMessage
{
    /// <summary>
    /// Gets or sets the namespace for the message.
    /// </summary>
    public string Namespace { get; init; }

    /// <summary>
    /// Gets or sets the key associated with the message.
    /// </summary>
    public string? Key { get; init; }

    /// <summary>
    /// Gets or sets the value of the message.
    /// </summary>
    public object? Value { get; init; }

    /// <summary>
    /// Gets or sets the type of the message.
    /// </summary>
    public MessageType Type { get; init; }

    /// <summary>
    /// Initializes a new instance of the RedisChannelMessage class with the specified parameters.
    /// </summary>
    /// <param name="Namespace">The namespace for the message.</param>
    /// <param name="Key">The key associated with the message.</param>
    /// <param name="Value">The value of the message.</param>
    /// <param name="Type">The type of the message.</param>
    public RedisChannelMessage(string Namespace, string? Key = null, object? Value = null, MessageType Type = MessageType.Set)
    {
        this.Namespace = Namespace;
        this.Key = Key;
        this.Value = Value;
        this.Type = Type;
    }
}