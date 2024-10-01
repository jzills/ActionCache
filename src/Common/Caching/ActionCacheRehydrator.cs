using ActionCache.Common.Extensions;
using ActionCache.Common.Extensions.Internal;
using ActionCache.Common.Keys;
using ActionCache.Common.Utilities;
using System.Text.Json;

namespace ActionCache.Common;

internal class ActionCacheRehydrator2
{
    protected readonly ActionCacheDescriptorProvider DescriptorProvider;
    protected readonly IActionCache Cache;

    public ActionCacheRehydrator2(
        IActionCache cache,
        ActionCacheDescriptorProvider descriptorProvider
    )
    {
        Cache = cache;
        DescriptorProvider = descriptorProvider;
    }

    public async Task<IReadOnlyCollection<RehydrationResult>> Rehydrate(string @namespace)
    {
        var rehydrationResults = new List<RehydrationResult>();
        var descriptorCollection = DescriptorProvider.GetControllerActionMethodInfo(@namespace);
        if (descriptorCollection.MethodInfos.Any())
        {
            var keys = await Cache.GetAsync<IEnumerable<string>>($"{nameof(ActionCache)}:{@namespace}");
            if (keys?.Any() ?? false)
            {
                foreach (var key in keys)
                {
                    var keyComponents = new ActionCacheKeyComponentsBuilder(key).Build();
                    var routeValuesKey = JsonSerializer.Serialize(keyComponents.RouteValues);
                    if (descriptorCollection.MethodInfos.TryGetValue(
                            routeValuesKey, 
                            out var methodInfo
                        ))
                    {
                        if (descriptorCollection.Controllers.TryGetValue(routeValuesKey, out var controller))
                        {
                            if (methodInfo.TryGetRehydrationResult(
                                    controller, 
                                    keyComponents.ActionArguments?.Values?.ToArray(), 
                                    out var result
                            ))
                            {
                                rehydrationResults.Add(result);
                            }
                        }
                    }
                }
            }
        }

        return rehydrationResults;
    }
}

/// <summary>
/// Provides functionality to rehydrate actions from cache.
/// </summary>
internal abstract class ActionCacheRehydrator : IActionCacheRehydrator
{
    /// <summary>
    /// Accessor property to retrieve action arguments.
    /// </summary>
    protected virtual Func<string, Task<IEnumerable<IDictionary<string, JsonElement>?>?>>? ActionArgsAccessor { get; init; }

    /// <summary>
    /// Descriptor provider for action cache.
    /// </summary>
    protected readonly ActionCacheDescriptorProvider DescriptorProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionCacheRehydrator"/> class.
    /// </summary>
    /// <param name="descriptorProvider">The descriptor provider instance.</param>
    public ActionCacheRehydrator(
        ActionCacheDescriptorProvider descriptorProvider
    ) => DescriptorProvider = descriptorProvider;

    /// <summary>
    /// Asynchronously gets rehydration results for a given namespace.
    /// </summary>
    /// <param name="namespace">The namespace to rehydrate.</param>
    /// <returns>A read-only collection of rehydration results.</returns>
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
                // Instead of ActionArgsAccessor, just use the IActionCache and get "ActionCache:Namespace" to 
                // fetch all of the keys currently in the cache. Then decrypt the keys to get the routeValues and actionArgs.
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