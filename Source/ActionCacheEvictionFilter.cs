using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ActionCache.Filters;

public class ActionCacheEvictionFilter : IAsyncActionFilter
{
    private IActionCache _cache;
    public ActionCacheEvictionFilter(IActionCache cache) => _cache = cache;
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var actionExecutedContext = await next();
        if (actionExecutedContext is not null)
        {
            if (actionExecutedContext.Result is OkObjectResult objectResult)
            {
                await _cache.RemoveAsync();
            }
        }
    }
}