using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ActionCache.Common.Extensions;

namespace ActionCache.Filters;

public class ActionCacheFilter : IAsyncActionFilter
{
    private readonly IActionCache _cache;
    public ActionCacheFilter(IActionCache cache) => _cache = cache;
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.TryGetKey(out var key))
        {
            var cacheValue = await _cache.GetAsync<object>(key);
            if (cacheValue is not null)
            {
                context.Result = new OkObjectResult(cacheValue);
                return;
            }

            var actionExecutedContext = await next();
            if (actionExecutedContext.TryGetObjectResultValue(out var value))
            {
                await _cache.SetAsync(key, value);
            }
        }
        else
        {
            await next();
        }
    }  
}
