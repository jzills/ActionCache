using ActionCache.Common.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ActionCache.Filters;

/// <summary>
/// Represents a filter to cache action results for improving performance.
/// </summary>
public class ActionCacheFilter : IAsyncActionFilter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ActionCacheFilter"/> class.
    /// </summary>
    /// <param name="cache">The cache implementation to use.</param>
    public ActionCacheFilter(IActionCache cache) => Cache = cache;

    /// <summary>
    /// The cache facility to use for caching action results.
    /// </summary>
    protected readonly IActionCache Cache;

    /// <summary>
    /// Called asynchronously before the action, after model binding is complete.
    /// </summary>
    /// <param name="context">The action executing context.</param>
    /// <param name="next">The action execution delegate. Invoked to execute the next action filter or the action itself.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.TryGetKey(out var key))
        {
            var cacheValue = await Cache.GetAsync<object>(key);
            if (cacheValue is not null)
            {
                context.Result = new OkObjectResult(cacheValue);
                return;
            }

            var actionExecutedContext = await next();
            if (actionExecutedContext.TryGetOkObjectResultValue(out var value))
            {
                await Cache.SetAsync(key, value);

                if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                {
                    // TODO: Rehydration
                }
            }
        }
        else
        {
            await next();
        }
    }
}