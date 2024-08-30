using ActionCache.Common.Extensions;
using ActionCache.Common.Extensions.Internal;
using ActionCache.Common.Utilities;
using System.Text.Json;

namespace ActionCache.Common
{
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
}