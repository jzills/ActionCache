using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ActionCache.Attributes;
using Microsoft.AspNetCore.Mvc.Abstractions;
using ActionCache.Common.Utilities;
using Microsoft.AspNetCore.Routing;

namespace ActionCache.Common.Extensions;

internal static class ActionExecutingContextExtensions
{
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

    internal static bool TryGetKey(this ActionExecutingContext context, out string key) 
    {
        var (actionDescriptor, routeData, actionArgs) = context;
        var keyInternal = new ActionCacheKeyBuilder(actionDescriptor)
            .WithRouteData(routeData)
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