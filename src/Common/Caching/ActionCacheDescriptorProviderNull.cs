using ActionCache.Utilities;

namespace ActionCache.Common.Caching;

/// <summary>
/// Provides functionality to retrieve and cache action descriptor information. This class
/// implements a no-op variation. The result will always be empty. 
/// </summary>
public class ActionCacheDescriptorProviderNull : IActionCacheDescriptorProvider
{
    /// <inheritdoc/>
    public ActionCacheDescriptor GetControllerActionMethodInfo(Namespace @namespace) => new ActionCacheDescriptor();

    /// <inheritdoc/>
    public string CreateKey(string? areaName, string? controllerName, string? actionName) => string.Empty;
}