using ActionCache.Utilities;

namespace ActionCache.Common.Caching;

public interface IActionCacheDescriptorProvider
{
    ActionCacheDescriptor GetControllerActionMethodInfo(Namespace @namespace);
    string CreateKey(string? areaName, string? controllerName, string? actionName);
}