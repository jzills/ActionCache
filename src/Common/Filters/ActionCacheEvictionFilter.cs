using ActionCache.Common.Enums;
using ActionCache.Common.Extensions;
using ActionCache.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing.Template;

namespace ActionCache.Filters;

/// <summary>
/// An action filter to handle cache eviction after successful action execution.
/// </summary>
public class ActionCacheEvictionFilter : IAsyncActionFilter
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
    /// Initializes a new instance of the <see cref="ActionCacheEvictionFilter"/> class.
    /// </summary>
    /// <param name="cache">The cache service used for removing cache entries.</param>
    /// <param name="binderFactory">The template binder for parsing route parameters for templated namespaces.</param>
    public ActionCacheEvictionFilter(
        IActionCache cache, 
        TemplateBinderFactory binderFactory
    )
    {
        Cache = cache;
        BinderFactory = binderFactory;
    }

    /// <summary>
    /// Executes asynchronously before and after the action method is invoked.
    /// </summary>
    /// <param name="context">The context for the executing action.</param>
    /// <param name="next">The delegate to execute the next stage in the action's execution pipeline.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var actionExecutedContext = await next();
        
        // Cache eviction logic after a successful response.
        if (actionExecutedContext.HttpContext.Response.StatusCode == StatusCodes.Status200OK)
        {
            Cache.GetNamespace().AttachRouteValues(
                context.RouteData.Values, 
                BinderFactory
            );

            context.AddCacheStatus(CacheStatus.EVICT);
            await Cache.RemoveAsync();
        }
    }
}