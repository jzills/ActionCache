using ActionCache.Attributes;
using ActionCache.Common.Utilities;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace ActionCache.Common.Extensions;

/// <summary>
/// Provides extensions for <see cref="ActionExecutingContext"/>.
/// </summary>
internal static class ActionExecutingContextExtensions
{
    /// <summary>
    /// Tries to retrieve key attributes from the action descriptor.
    /// </summary>
    /// <param name="actionDescriptor">The action descriptor from which to extract key attributes.</param>
    /// <param name="attributes">Outputs a read-only dictionary of key attributes if found.</param>
    /// <returns>True if attributes are found, otherwise false.</returns>
    internal static bool TryGetKeyAttributes(
        this ActionDescriptor actionDescriptor, 
        out IReadOnlyDictionary<string, ActionCacheKeyAttribute> attributes
    )
    {
        var attributesInternal = new Dictionary<string, ActionCacheKeyAttribute>();
        if (actionDescriptor.Parameters is not null)
        {
            foreach (var parameter in actionDescriptor.Parameters)
            {
                if (parameter is ControllerParameterDescriptor controllerParameter)
                {
                    var attribute = controllerParameter.ParameterInfo.GetCustomAttribute<ActionCacheKeyAttribute>();
                    if (attribute is not null)
                    {
                        attributesInternal.TryAdd(parameter.Name, attribute);
                    }
                }
            }
        }

        attributes = attributesInternal.AsReadOnly();
        return attributesInternal.Count > 0;
    }

    /// <summary>
    /// Tries to generate a cache key from the given context.
    /// </summary>
    /// <param name="context">The action executing context containing necessary data.</param>
    /// <param name="key">Outputs the generated cache key.</param>
    /// <returns>True if a key is successfully generated, otherwise false.</returns>
    internal static bool TryGetKey(this ActionExecutingContext context, out string key) 
    {
        var (actionDescriptor, routeData, actionArgs) = context;
        var keyInternal = new ActionCacheKeyBuilder()
            .WithRouteValues(routeData.Values)
            .WithActionArguments(actionArgs)
            .Build();

        if (string.IsNullOrWhiteSpace(keyInternal))
        {
            key = default!;
            return false;
        }

        key = keyInternal;
        return true;
    }

    /// <summary>
    /// Deconstructs the action executing context to its core components.
    /// </summary>
    /// <param name="context">The context to deconstruct.</param>
    /// <param name="actionDescriptor">Outputs the action descriptor.</param>
    /// <param name="routeData">Outputs the routing data.</param>
    /// <param name="actionArgs">Outputs the action arguments dictionary.</param>
    internal static void Deconstruct(
        this ActionExecutingContext context, 
        out ActionDescriptor actionDescriptor, 
        out RouteData routeData, 
        out IDictionary<string, object> actionArgs
    )
    {
        actionDescriptor = context.ActionDescriptor;
        routeData = context.RouteData;
        actionArgs = context.ActionArguments;
    }
}