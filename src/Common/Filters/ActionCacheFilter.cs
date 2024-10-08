using ActionCache.Common.Enums;
using ActionCache.Common.Extensions;
using ActionCache.Common.Extensions.Internal;
using ActionCache.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing.Template;

namespace ActionCache.Filters;

/// <summary>
/// Represents a filter to cache action results for improving performance.
/// </summary>
public class ActionCacheFilter : IAsyncActionFilter
{
    /// <summary>
    /// The cache facility to use for caching action results.
    /// </summary>
    protected readonly IActionCache Cache;

    /// <summary>
    /// The template binder for parsing route parameters for templated namespaces.
    /// </summary>
    protected readonly TemplateBinderFactory BinderFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionCacheFilter"/> class.
    /// </summary>
    /// <param name="cache">The cache implementation to use.</param>
    public ActionCacheFilter(
        IActionCache cache, 
        TemplateBinderFactory binderFactory
    )
    {
        Cache = cache;
        BinderFactory = binderFactory;
    }

    /// <summary>
    /// Called asynchronously before the action, after model binding is complete.
    /// </summary>
    /// <param name="context">The action executing context.</param>
    /// <param name="next">The action execution delegate. Invoked to execute the next action filter or the action itself.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        Cache.GetNamespace().AttachRouteValues(
            context.RouteData.Values, 
            BinderFactory
        );

        if (context.TryGetKey(out var key))
        {
            var cacheValue = await Cache.GetAsync<object>(key);
            if (cacheValue is not null)
            {
                context.AddCacheStatus(CacheStatus.HIT);
                context.Result = new OkObjectResult(cacheValue);
            }
            else
            {
                var actionExecutedContext = await next();
                if (actionExecutedContext.TryGetOkObjectResultValue(out var value))
                {
                    context.AddCacheStatus(CacheStatus.ADD);
                    await Cache.SetAsync(key, value);
                }
                else
                {
                    context.AddCacheStatus(CacheStatus.NONE);
                }
            }
        }
        else
        {
            context.AddCacheStatus(CacheStatus.MISS);
            await next();
        }
    }
}