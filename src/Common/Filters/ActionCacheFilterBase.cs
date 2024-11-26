using ActionCache.Common.Extensions.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;

namespace ActionCache.Filters;

/// <summary>
/// The abstract base class for <see cref="ActionCacheFilter"/>.
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

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionCacheFilterBase"/> class with the specified cache and template binder factory.
    /// </summary>
    /// <param name="cache">The <see cref="IActionCache"/> instance used for caching actions.</param>
    /// <param name="binderFactory">The <see cref="TemplateBinderFactory"/> instance used for binding route templates.</param>
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