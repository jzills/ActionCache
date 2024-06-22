using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Controllers;
using ActionCache.Common.Extensions;

namespace ActionCache.Filters;

public class ActionCacheFilter : IAsyncActionFilter
{
    protected readonly IActionCache Cache;
    public ActionCacheFilter(IActionCache cache) => Cache = cache;

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
