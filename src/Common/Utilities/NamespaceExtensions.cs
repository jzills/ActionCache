using System.Web;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.AspNetCore.Routing.Template;

namespace ActionCache.Utilities;

public static class NamespaceExtensions
{
    public static bool ContainsRouteTemplateParameters(this Namespace source) =>
        RoutePatternFactory.Parse(source)?.Parameters?.Any() ?? false;

    public static void AttachRouteValues(this Namespace source, 
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
                // Remove the first forward slash and any query string parameters
                boundValues = boundValues.Substring(1, boundValues.LastIndexOf("?") - 1);
            }

            source.ValueWithRouteTemplateParameters = HttpUtility.UrlDecode(boundValues);
        }
    }
}