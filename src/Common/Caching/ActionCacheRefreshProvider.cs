using System.Text.Json;
using ActionCache.Common.Extensions.Internal;
using ActionCache.Common.Keys;
using ActionCache.Common.Utilities;

namespace ActionCache.Caching;

public class ActionCacheRefreshProvider
{
    protected readonly IActionCache Cache;
    protected readonly ActionCacheDescriptorProvider DescriptorProvider;

    public ActionCacheRefreshProvider(
        IActionCache cache,
        ActionCacheDescriptorProvider descriptorProvider
    )
    {
        Cache = cache;
        DescriptorProvider = descriptorProvider;
    }

    public async Task RefreshAsync()
    {
        var descriptorCollection = DescriptorProvider.GetControllerActionMethodInfo(Cache.GetNamespace());
        if (descriptorCollection.MethodInfos.Any())
        {
            var keys = await Cache.GetKeysAsync();
            if (keys.Some())
            {
                foreach (var key in keys)
                {
                    // Recreate the key components from the encrypted key value
                    var keyComponents = new ActionCacheKeyComponentsBuilder(key).Build();

                    // Deconstruct the route values used as a key into the methodInfo
                    // for a given controller action
                    var (areaName, controllerName, actionName) = keyComponents;
                    var routeValuesKey = DescriptorProvider.CreateKey(areaName, controllerName, actionName);

                    if (descriptorCollection.MethodInfos.TryGetValue(
                            routeValuesKey, 
                            out var methodInfo
                        ))
                    {
                        if (descriptorCollection.Controllers.TryGetValue(routeValuesKey, out var controller))
                        {
                            if (methodInfo.TryGetRefreshResult(
                                    controller, 
                                    keyComponents.ActionArguments?.Values?.ToArray(), 
                                    out var value
                            ))
                            {
                                await Cache.SetAsync(key, value);
                            }
                        }
                    }
                }
            }
        }
    }
}