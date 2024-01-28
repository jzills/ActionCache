using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ActionCache.Filters;

public class ActionCacheEvictionFilter : IAsyncActionFilter
{
    private IActionCache[] _caches;
    public ActionCacheEvictionFilter(params IActionCache[] caches) => _caches = caches;
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var actionExecutedContext = await next();
        if (actionExecutedContext is not null)
        {
            if (actionExecutedContext.Result is OkObjectResult objectResult)
            {
                var cacheTasks = _caches.Select(cache => cache.RemoveAsync());
                await Task.WhenAll(cacheTasks);
            }
        }
    }
}