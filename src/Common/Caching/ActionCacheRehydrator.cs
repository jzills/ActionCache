using System.Text.Json;
using ActionCache.Common.Extensions;
using ActionCache.Common.Utilities;
using ActionCache.Common.Extensions.Internal;

namespace ActionCache.Common;

internal abstract class ActionCacheRehydrator : IActionCacheRehydrator
{
    protected virtual Func<string, Task<IEnumerable<IDictionary<string, JsonElement>?>?>>? ActionArgsAccessor { get; init; }
    protected readonly ActionCacheDescriptorProvider DescriptorProvider;

    public ActionCacheRehydrator(
        ActionCacheDescriptorProvider descriptorProvider
    ) => DescriptorProvider = descriptorProvider;
    
    public async Task<IReadOnlyCollection<RehydrationResult>> GetRehydrationResultsAsync(string @namespace)
    {
        ArgumentNullException.ThrowIfNull(ActionArgsAccessor, nameof(ActionArgsAccessor));

        var rehydrationDescriptor = new List<RehydrationResult>();
        var descriptorCollection = DescriptorProvider.GetControllerActionMethodInfo(@namespace);
        if (descriptorCollection.MethodInfos.Any())
        {
            foreach (var (route, methodInfo) in descriptorCollection.MethodInfos)
            {
                var controller = descriptorCollection.Controllers[route];
                var actionArgs = await ActionArgsAccessor($"ActionCache:{@namespace}:Rehydration:{route}");
                if (actionArgs is not null)
                {
                    foreach (var actionArg in actionArgs)
                    {
                        var actionValueConversions = new SortedList<int, object?>();
                        foreach (var parameter in methodInfo.GetParameters())
                        {
                            if (parameter.TryGetValue(actionArg, out var actionCacheAttribute))
                            {
                                actionValueConversions.Add(
                                    actionCacheAttribute.Order, 
                                    actionCacheAttribute.Value
                                );
                            }
                        }

                        if (methodInfo.TryGetRehydrationResult( 
                                controller, 
                                actionValueConversions.Values.ToArray(), 
                                out var result
                        ))
                        {
                            rehydrationDescriptor.Add(result);
                        }
                    } 
                }    
            }
        }

        return rehydrationDescriptor.AsReadOnly();
    }
}