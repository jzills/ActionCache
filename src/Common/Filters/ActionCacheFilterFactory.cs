using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using ActionCache.Common;
using ActionCache.Common.Extensions;

namespace ActionCache.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ActionCacheFilterFactory : Attribute, IFilterFactory
{
    public required string Namespace { get; set; }
    public bool IsReusable => false;

    public virtual IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(Namespace, nameof(Namespace));

        if (serviceProvider.TryGetActionCaches(Namespace, out var caches))
        {
            return new ActionCacheFilter(
                new ActionCacheAggregate(caches),
                serviceProvider.GetRequiredService<IActionCacheRehydrator>());
        }
        else
        {
            // TODO: Test this...not sure what happens
            // when you return default or null from
            // IFilterFactory
            return default!;
        }
    }
}