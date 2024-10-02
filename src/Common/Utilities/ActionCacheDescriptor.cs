using System.Reflection;

namespace ActionCache.Common.Utilities;

/// <summary>
/// Represents a descriptor to hold method information and corresponding controller instances for action cache events.
/// </summary>
public class ActionCacheDescriptor
{
    /// <summary>
    /// A dictionary containing method information indexed by a unique key.
    /// </summary>
    public Dictionary<string, MethodInfo> MethodInfos = new();

    /// <summary>
    /// A dictionary containing controller instances indexed by a unique key.
    /// </summary>
    public Dictionary<string, object> Controllers = new();

    /// <summary>
    /// Adds method information and a corresponding controller instance to the descriptor.
    /// </summary>
    /// <param name="key">The unique key to identify the method and controller.</param>
    /// <param name="methodInfo">The MethodInfo object representing the method.</param>
    /// <param name="controller">The controller instance associated with the method.</param>
    /// <exception cref="ArgumentException">Thrown when the key is null or whitespace.</exception>
    public void Add(string key, MethodInfo methodInfo, object controller)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));

        MethodInfos.Add(key, methodInfo);
        Controllers.Add(key, controller);
    }
}