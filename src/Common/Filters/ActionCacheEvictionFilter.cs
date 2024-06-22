using ActionCache.Common.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ActionCache.Filters;

public class ActionCacheEvictionFilter : IAsyncActionFilter
{
    protected readonly IActionCache Cache;
    public ActionCacheEvictionFilter(IActionCache cache) => Cache = cache;
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var actionExecutedContext = await next();
        if (actionExecutedContext.TryGetOkObjectResultValue(out _))
        {
            await Cache.RemoveAsync();
        }
    }
}