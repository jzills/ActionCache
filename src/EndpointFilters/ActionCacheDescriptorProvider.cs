using ActionCache.Common.Caching;
using ActionCache.Common.Extensions.Internal;
using ActionCache.Utilities;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace ActionCache.EndpointFilters;

/// <summary>
/// Provides functionality to retrieve and cache endpoint method information.
/// </summary>
public class ActionCacheDescriptorProvider : IActionCacheDescriptorProvider
{
    /// <summary>
    /// Service provider for obtaining services.
    /// </summary>
    protected readonly IServiceProvider ServiceProvider;

    /// <summary>
    /// Data source for Minimal API and MVC endpoints.
    /// </summary>
    protected readonly EndpointDataSource EndpointDataSource;

    /// <summary>
    /// Builder for creating cache keys.
    /// </summary>
    protected readonly StringBuilder KeyBuilder;

    /// <summary>
    /// Constructs an ActionCacheDescriptor with a service provider and endpoint data source.
    /// </summary>
    /// <param name="serviceProvider">Service provider for dependency resolution.</param>
    /// <param name="endpointDataSource">Data source for Minimal API and MVC endpoints.</param>
    public ActionCacheDescriptorProvider(IServiceProvider serviceProvider, EndpointDataSource endpointDataSource)
    {
        ServiceProvider = serviceProvider;
        EndpointDataSource = endpointDataSource;
        KeyBuilder = new StringBuilder();
    }

    /// <summary>
    /// Retrieves the method info associated with all registered endpoints (Minimal APIs & MVC Controllers).
    /// </summary>
    /// <returns>A cache descriptor containing method info.</returns>
    public ActionCacheDescriptor GetControllerActionMethodInfo(Namespace @namespace)//GetAllEndpointMethodInfo()
    {
        var descriptors = new ActionCacheDescriptor();

        foreach (var endpoint in EndpointDataSource.Endpoints.OfType<RouteEndpoint>())
        {
            var metadata = endpoint.Metadata;

            var controllerDescriptor = metadata.OfType<ControllerActionDescriptor>().FirstOrDefault();
            if (controllerDescriptor is not null)
            {
                var controller = ServiceProvider.GetRequiredService(controllerDescriptor.ControllerTypeInfo);
                var methodInfo = controllerDescriptor.MethodInfo;

                if (methodInfo.HasActionCacheAttribute(@namespace))
                {
                    var key = CreateKey(controllerDescriptor.ControllerName, controllerDescriptor.ActionName);
                    descriptors.Add(key, methodInfo, controller);
                }

                continue; 
            }

            var methodInfoFromMinimalApi = endpoint.RequestDelegate?.Method;
            if (methodInfoFromMinimalApi is not null)
            {
                if (methodInfoFromMinimalApi.HasActionCacheAttribute(@namespace))
                {
                    var key = CreateKey("MinimalAPI", endpoint.RoutePattern.RawText);
                    descriptors.Add(key, methodInfoFromMinimalApi, null);
                }
            }
        }

        return descriptors;
    }

    /// <summary>
    /// Creates a cache key based on the controller name and action name or route pattern.
    /// </summary>
    /// <param name="controllerName">Controller name of the action (or "MinimalAPI" for Minimal APIs).</param>
    /// <param name="actionOrRoute">Action name (for controllers) or route pattern (for Minimal APIs).</param>
    /// <returns>A string key for caching.</returns>
    public string CreateKey(string controllerName, string actionOrRoute, string _ = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(controllerName, nameof(controllerName));
        ArgumentException.ThrowIfNullOrWhiteSpace(actionOrRoute, nameof(actionOrRoute));

        var key = KeyBuilder.AppendJoinNonNull(':', controllerName, actionOrRoute).ToString();
        KeyBuilder.Clear();
        return key;
    }
}
