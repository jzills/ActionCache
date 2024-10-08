using ActionCache.Common.Enums;
using ActionCache.Common.Extensions;
using ActionCache.Common.Extensions.Internal;
using ActionCache.Utilities;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing.Template;

namespace ActionCache.Filters;

/// <summary>
/// Provides a filter for refreshing action caches.
/// </summary>
internal class ActionCacheRefreshFilter : IAsyncResultFilter
{
    /// <summary>
    /// The action cache manager.
    /// </summary>
    protected readonly IActionCache Cache;

    /// <summary>
    /// The template binder for parsing route parameters for templated namespaces.
    /// </summary>
    protected readonly TemplateBinderFactory BinderFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionCacheRefreshFilter"/> class.
    /// </summary>
    /// <param name="cache">The cache service used for refreshing cache entries.</param>
    /// <param name="binderFactory">The template binder for parsing route parameters for templated namespaces.</param>
    public ActionCacheRefreshFilter(
        IActionCache cache, 
        TemplateBinderFactory binderFactory
    )
    {
        Cache = cache;
        BinderFactory = binderFactory;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionCacheRefreshFilter"/> class.
    /// </summary>
    /// <param name="cache">The action cache.</param>
    public ActionCacheRefreshFilter(IActionCache cache) => Cache = cache;
    
    /// <summary>
    /// Asynchronously executes the result operation with cache refresh.
    /// </summary>
    /// <param name="context">The context for result executing.</param>
    /// <param name="next">Delegate for the result execution.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task OnResultExecutionAsync(
        ResultExecutingContext context, 
        ResultExecutionDelegate next
    )
    {
        Cache.GetNamespace().AttachRouteValues(
            context.RouteData.Values, 
            BinderFactory
        );
        
        context.AddCacheStatus(CacheStatus.REFRESH);
        await Cache.RefreshAsync();
        await next();
    }
}