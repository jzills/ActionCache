using ActionCache.Common.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ActionCache.Filters;

public class ActionCacheEvictionFilter : IAsyncActionFilter
{
    private readonly IActionCache _cache;
    public ActionCacheEvictionFilter(IActionCache cache) => _cache = cache;
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var actionExecutedContext = await next();
        if (actionExecutedContext.TryGetOkObjectResultValue(out _))
        {
            await _cache.RemoveAsync();
        }
    }
}