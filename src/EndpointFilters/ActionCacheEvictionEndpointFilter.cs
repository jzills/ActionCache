using ActionCache.Common.Enums;
using ActionCache.Common.Extensions.Internal;
using ActionCache.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;

namespace ActionCache.EndpointFilters;

public class ActionCacheEvictionEndpointFilter : ActionCacheFilterBase, IEndpointFilter
{
    public ActionCacheEvictionEndpointFilter(
        IActionCache cache, 
        TemplateBinderFactory binderFactory
    ) : base(cache, binderFactory)
    {
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var result = await next(context);
        
        if (context.HttpContext.Response.StatusCode == StatusCodes.Status200OK)
        {
            AttachRouteValues(context.HttpContext.GetRouteData().Values);
            context.HttpContext.Response.Headers.AddCacheStatus(CacheStatus.Evict);

            await Cache.RemoveAsync();
        }

        return result;
    }
}