using ActionCache.Utilities;

namespace ActionCache.Common.Caching;

/// <summary>
/// Provides functionality for describing and managing action cache metadata.
/// </summary>
public interface IActionCacheDescriptorProvider
{
    /// <summary>
    /// Retrieves the action cache descriptor for a specified controller action within the given namespace.
    /// </summary>
    /// <param name="namespace">The namespace containing the controller action.</param>
    /// <returns>
    /// An <see cref="ActionCacheDescriptor"/> containing metadata about the specified controller action.
    /// </returns>
    ActionCacheDescriptor GetControllerActionMethodInfo(Namespace @namespace);

    /// <summary>
    /// Creates a unique cache key based on the specified area, controller, and action names.
    /// </summary>
    /// <param name="areaName">The name of the area. Can be null.</param>
    /// <param name="controllerName">The name of the controller. Can be null.</param>
    /// <param name="actionName">The name of the action. Can be null.</param>
    /// <returns>
    /// A string representing a unique cache key derived from the provided names.
    /// </returns>
    string CreateKey(string? areaName, string? controllerName, string? actionName);
}