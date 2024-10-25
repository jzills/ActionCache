using ActionCache.Utilities;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;

namespace ActionCache.Filters;

/// <summary>
/// The 
/// </summary>
public abstract class ActionCacheFilterBase
{
    /// <summary>
    /// An instance of an implementation of an IActionCache.
    /// </summary>
    protected readonly IActionCache Cache;

    /// <summary>
    /// The template binder for parsing route parameters for templated namespaces.
    /// </summary>
    protected readonly TemplateBinderFactory BinderFactory;

    internal ActionCacheFilterBase(IActionCache cache, TemplateBinderFactory binderFactory)
    {
        Cache = cache;
        BinderFactory = binderFactory;
    }

    /// <summary>
    /// Attaches any route values to a namespace that contains route template placeholders.
    /// </summary>
    /// <param name="routeValues">A dictionary of route values.</param>
    protected void AttachRouteValues(RouteValueDictionary routeValues)
    {
        var @namespace = Cache.GetNamespace();
        @namespace.AttachRouteValues(routeValues, BinderFactory);
    }
}