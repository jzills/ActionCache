using ActionCache.Common.Extensions.Internal;
using ActionCache.Common.Keys;

namespace ActionCache.Common.Caching;

public class ActionCacheRefreshProvider
{
    protected readonly ActionCacheDescriptorProvider DescriptorProvider;

    public ActionCacheRefreshProvider(ActionCacheDescriptorProvider descriptorProvider)
    {
        DescriptorProvider = descriptorProvider;
    }

    public async Task<IReadOnlyDictionary<string, object?>> GetRefreshResultsAsync(string @namespace, IEnumerable<string> keys)
    {
        var refreshResults = new Dictionary<string, object?>();
        var descriptorCollection = DescriptorProvider.GetControllerActionMethodInfo(@namespace);
        if (descriptorCollection.MethodInfos.Any())
        {
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
                                refreshResults.Add(key, value);
                            }
                        }
                    }
                }
            }
        }

        return refreshResults;
    }
}