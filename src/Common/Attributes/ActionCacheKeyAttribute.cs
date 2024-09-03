namespace ActionCache.Attributes;

/// <summary>
/// Attribute to mark a parameter as part of the cache key in an action method, allowing customization of its order of importance.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class ActionCacheKeyAttribute : Attribute
{
    /// <summary>
    /// Gets the order of the parameter in cache key generation.
    /// </summary>
    public int Order { get; init; }
}