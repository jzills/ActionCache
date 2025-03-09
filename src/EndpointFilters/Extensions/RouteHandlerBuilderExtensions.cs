using ActionCache.Attributes;
using ActionCache.Filters;
using ActionCache.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.DependencyInjection;

namespace ActionCache.EndpointFilters.Extensions;

public static class RouteHandlerBuilderExtensions
{
    public static RouteHandlerBuilder WithActionCache(this RouteHandlerBuilder builder, Namespace @namespace) =>
        builder
            .WithMetadata(new ActionCacheAttribute { Namespace = @namespace })
            .AddEndpointFilter((context, next) =>
            {
                var endpoint = context.HttpContext.GetEndpoint();
                if (endpoint is null)
                {
                    return next(context);
                }
                else
                {
                    var attribute = endpoint.Metadata.GetMetadata<ActionCacheAttribute>();
                    var cacheFactory = context.HttpContext.RequestServices.GetRequiredService<IActionCacheFactory>();
                    var templateBinderFactory = context.HttpContext.RequestServices.GetRequiredService<TemplateBinderFactory>();
                    var cache = cacheFactory.Create(attribute.Namespace);
                    return new ActionCacheEndpointFilter(cache, templateBinderFactory)
                        .InvokeAsync(context, next);
                }
            });

    public static RouteHandlerBuilder WithActionCacheEviction(this RouteHandlerBuilder builder, Namespace @namespace) =>
        builder
            .WithMetadata(new ActionCacheEvictionAttribute { Namespace  = @namespace })
            .AddEndpointFilter((context, next) =>
            {
                var endpoint = context.HttpContext.GetEndpoint();
                if (endpoint is null)
                {
                    return next(context);
                }
                else
                {
                    var attribute = endpoint.Metadata.GetMetadata<ActionCacheEvictionAttribute>();
                    var cacheFactory = context.HttpContext.RequestServices.GetRequiredService<IActionCacheFactory>();
                    var templateBinderFactory = context.HttpContext.RequestServices.GetRequiredService<TemplateBinderFactory>();
                    var cache = cacheFactory.Create(attribute.Namespace);
                    return new ActionCacheEvictionEndpointFilter(cache, templateBinderFactory)
                        .InvokeAsync(context, next);
                }
            });
}