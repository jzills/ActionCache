using ActionCache.Common.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ActionCache.Filters;

/// <summary>
/// An action filter to handle cache eviction after successful action execution.
/// </summary>
public class ActionCacheEvictionFilter : IAsyncActionFilter
{
    /// <summary>
    /// The cache instance.
    /// </summary>
    protected readonly IActionCache Cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionCacheEvictionFilter"/> class.
    /// </summary>
    /// <param name="cache">The cache service used for removing cache entries.</param>
    public ActionCacheEvictionFilter(IActionCache cache) => Cache = cache;

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
        if (actionExecutedContext.TryGetOkObjectResultValue(out _))
        {
            await Cache.RemoveAsync();
        }
    }
}