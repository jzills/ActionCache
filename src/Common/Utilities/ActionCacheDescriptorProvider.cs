using ActionCache.Common.Extensions.Internal;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Text;

namespace ActionCache.Common.Utilities;

/// <summary>
/// Provides functionality to retrieve and cache action descriptor information.
/// </summary>
public class ActionCacheDescriptorProvider
{
    /// <summary>
    /// Static cache for action rehydration descriptors.
    /// </summary>
    protected static readonly IDictionary<string, ActionCacheDescriptor> Cache;

    /// <summary>
    /// Service provider for obtaining services.
    /// </summary>
    protected readonly IServiceProvider ServiceProvider;

    /// <summary>
    /// Collection of action descriptors.
    /// </summary>
    protected readonly ActionDescriptorCollection ActionDescriptors;
    
    /// <summary>
    /// Builder for creating cache keys.
    /// </summary>
    protected readonly StringBuilder KeyBuilder;

    /// <summary>
    /// Initializes the static cache.
    /// </summary>
    static ActionCacheDescriptorProvider()
    {
        Cache = new ConcurrentDictionary<string, ActionCacheDescriptor>();
    }

    /// <summary>
    /// Constructs an ActionCacheDescriptor with a service provider and descriptor provider.
    /// </summary>
    /// <param name="serviceProvider">Service provider for dependency resolution.</param>
    /// <param name="descriptorProvider">Provider of action descriptors.</param>
    public ActionCacheDescriptorProvider(
        IServiceProvider serviceProvider,
        IActionDescriptorCollectionProvider descriptorProvider
    )
    {
        ServiceProvider = serviceProvider;
        ActionDescriptors = descriptorProvider.ActionDescriptors;
        KeyBuilder = new StringBuilder();
    }

    /// <summary>
    /// Retrieves the action method info associated with a namespace.
    /// </summary>
    /// <param name="namespace">The namespace of the controller.</param>
    /// <returns>A rehydration descriptor for the specified namespace.</returns>
    public ActionCacheDescriptor GetControllerActionMethodInfo(string @namespace)
    {
        if (Cache.TryGetValue(@namespace, out var cacheDescriptors))
        {
            return cacheDescriptors;
        }
        else
        {
            var descriptors = new ActionCacheDescriptor();

            if (ActionDescriptors.TryGetControllerActionDescriptors(@namespace, out var controllerActionDescriptors))
            {
                foreach (var (areaName, controllerName, actionName, controllerTypeInfo) in controllerActionDescriptors)
                {
                    var controller = ServiceProvider.GetRequiredService(controllerTypeInfo);
                    var methodInfo = controller.GetType().GetMethod(actionName);
                    if (methodInfo is not null)
                    {
                        var key = CreateKey(areaName, controllerName, actionName);
                        descriptors.Add(key, methodInfo, controller);
                    }
                }
            }

            Cache.TryAdd(@namespace, descriptors);

            return descriptors;
        }
    }

    /// <summary>
    /// Creates a cache key based on the area name, controller name, and action name.
    /// </summary>
    /// <param name="areaName">Area name of the controller action.</param>
    /// <param name="controllerName">Controller name of the action.</param>
    /// <param name="actionName">Name of the action method.</param>
    /// <returns>A string key for the cache.</returns>
    public string CreateKey(string? areaName, string controllerName, string actionName)
    {
        var key = KeyBuilder.AppendJoinNonNull(':', areaName, controllerName, actionName).ToString();
        KeyBuilder.Clear();
        return key;
    }
}