namespace ActionCache.Utilities;

/// <summary>
/// Defines message types for actions that can be performed on a cache.
/// </summary>
public enum MessageType
{
    /// <summary>
    /// Set the cache value.
    /// </summary>
    Set,
    
    /// <summary>
    /// Remove the item from the cache by its key.
    /// </summary>
    RemoveByKey,
    
    /// <summary>
    /// Remove items from the cache by their namespace.
    /// </summary>
    RemoveByNamespace
}