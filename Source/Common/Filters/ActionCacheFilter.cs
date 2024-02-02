using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ActionCache.Attributes;

namespace ActionCache.Filters;

public class ActionCacheFilter : IAsyncActionFilter
{
    private IActionCache _cache;
    public ActionCacheFilter(IActionCache cache) => _cache = cache;
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var key = GetKey(context);
        if (!string.IsNullOrWhiteSpace(key))
        {
            var cacheValue = await _cache.GetAsync<object>(key);
            if (cacheValue is not null)
            {
                context.Result = new OkObjectResult(cacheValue);
                return;
            }
        }
        
        var actionExecutedContext = await next();
        if (!string.IsNullOrWhiteSpace(key))
        {
            if (actionExecutedContext is not null)
            {
                if (actionExecutedContext.Result is OkObjectResult objectResult)
                {
                    await _cache.SetAsync(key, objectResult.Value);
                }
            }
        }
    }

    private string? GetKey(ActionExecutingContext context) 
    {
        var parameters = context.ActionDescriptor.Parameters;
        var keyAttributes = new Dictionary<string, ActionCacheKeyAttribute>();
        foreach (var parameter in parameters)
        {
            if (parameter is ControllerParameterDescriptor controllerParameter)
            {
                var keyAttribute = controllerParameter.ParameterInfo
                    .GetCustomAttributes(false)
                    .OfType<ActionCacheKeyAttribute>()
                    .FirstOrDefault();

                if (keyAttribute is not null)
                {
                    keyAttributes[parameter.Name] = keyAttribute;
                }
            }
        }

        if (context.ActionArguments.Any())
        {
            var actionArguments = keyAttributes
                .OrderBy(attribute => attribute.Value.Order)
                .Select(attribute => context.ActionArguments[attribute.Key]);

            return string.Join(":", actionArguments);
        }
        else
        {
            return null;
        }
    }
}
