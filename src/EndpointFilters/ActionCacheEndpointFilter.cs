using ActionCache.Common.Enums;
using ActionCache.Common.Extensions;
using ActionCache.Common.Extensions.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using ActionCache.MinimalApi.Extensions.Internal;

namespace ActionCache.Filters;

/// <summary>
/// Represents a filter to cache action results for improving performance.
/// </summary>
public class ActionCacheEndpointFilter : ActionCacheFilterBase, IEndpointFilter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ActionCacheFilter"/> class.
    /// </summary>
    /// <param name="cache">The cache implementation to use.</param>
    /// <param name="binderFactory">The binder used for namespaces with route templates.</param>
    public ActionCacheEndpointFilter(
        IActionCache cache, 
        TemplateBinderFactory binderFactory
    ) : base(cache, binderFactory)
    {
    }

    /// <summary>
    /// Called asynchronously before the action, after model binding is complete.
    /// </summary>
    /// <param name="context">The action executing context.</param>
    /// <param name="next">The action execution delegate. Invoked to execute the next action filter or the action itself.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        AttachRouteValues(context.HttpContext.GetRouteData().Values);

        if (context.TryGetKey(out var key))
        {
            var cacheValue = await Cache.GetAsync<object?>(key);
            if (cacheValue is not null)
            {
                context.AddCacheStatus(CacheStatus.Hit);
                return cacheValue;
            }
            else
            {
                var result = await next(context);
                if (result is not null)
                {
                    context.AddCacheStatus(CacheStatus.Add);
                    await Cache.SetAsync(key, result);
                }
                else
                {
                    context.AddCacheStatus(CacheStatus.None);
                }
                
                return result;
            }
        }
        else
        {
            context.AddCacheStatus(CacheStatus.Miss);
            return await next(context);
        }
    }
}