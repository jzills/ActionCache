using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ActionCache.Common.Extensions;
using Microsoft.AspNetCore.Mvc.Controllers;
using ActionCache.Common;

namespace ActionCache.Filters;

public class ActionCacheFilter : IAsyncActionFilter
{
    private readonly IActionCache _cache;
    private readonly IActionCacheRehydrator _cacheRehydrator;
    public ActionCacheFilter(
        IActionCache cache,
        IActionCacheRehydrator cacheRehydrator
    ) => (_cache, _cacheRehydrator) = (cache, cacheRehydrator);

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

                if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                {
                    // TODO: Get area
                    var rehydrationKey = $"ActionCache:Rehydration:{controllerActionDescriptor.ControllerName}:{controllerActionDescriptor.ActionName}";
                    await _cacheRehydrator.SetAsync(rehydrationKey, context.ActionArguments);
                }
            }
        }
        else
        {
            await next();
        }
    }  
}
