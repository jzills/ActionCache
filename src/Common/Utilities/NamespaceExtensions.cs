using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.AspNetCore.Routing.Template;

namespace ActionCache.Utilities;

internal static class NamespaceExtensions
{
    internal static bool ContainsRouteTemplateParameters(this Namespace source) =>
        RoutePatternFactory.Parse(source)?.Parameters?.Any() ?? false;

    internal static void AttachRouteValues(this Namespace source, 
        RouteValueDictionary routeValues, 
        TemplateBinderFactory templateBinderFactory
    )
    {
        if (source.ContainsRouteTemplateParameters())
        {
            var templateBinder = templateBinderFactory.Create(
                TemplateParser.Parse(source.Value), 
                new RouteValueDictionary()
            );

            var boundValues = templateBinder.BindValues(routeValues);
            if (boundValues?.Contains("?") ?? false)
            {
                boundValues = boundValues.Substring(0, boundValues.LastIndexOf("?"));
            }

            source.ValueWithRouteTemplateParameters = boundValues;
        }
    }
}