using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ActionCache.Attributes;

namespace ActionCache.Common.Extensions;

internal static class ActionExecutingContextExtensions
{
    internal static bool TryGetKeyAttributes(
        this ActionExecutingContext context, 
        out Dictionary<string, ActionCacheKeyAttribute> attributes
    )
    {
        attributes = new();

        if (context.ActionDescriptor.Parameters is not null)
        {
            foreach (var parameter in context.ActionDescriptor.Parameters)
            {
                if (parameter is ControllerParameterDescriptor controllerParameter)
                {
                    var attribute = controllerParameter.ParameterInfo
                        .GetCustomAttribute<ActionCacheKeyAttribute>();

                    if (attribute is not null)
                    {
                        attributes.TryAdd(parameter.Name, attribute);
                    }
                }
            }
        }

        return attributes.Count > 0;
    }

    internal static bool TryGetKey(this ActionExecutingContext context, out string key) 
    {
        if (context.TryGetKeyAttributes(out var attributes) && 
            context.ActionArguments.Any())
        {
            var args = attributes.GetArguments(context.ActionArguments);
            key = string.Join(":", args);
            return !string.IsNullOrWhiteSpace(key);
        }
        else if (context.RouteData.Values.Any())
        {
            key = string.Join(":", context.RouteData.Values.Select(route => route.Value));
            return !string.IsNullOrWhiteSpace(key);
        }
        else
        {
            key = default!;
            return false;
        }
    }
}