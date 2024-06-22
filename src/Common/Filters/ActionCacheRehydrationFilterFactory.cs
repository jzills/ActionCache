using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using ActionCache.Common.Extensions;
using ActionCache.Common;

namespace ActionCache.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
internal class ActionCacheRehydrationFilterFactory : Attribute, IFilterFactory
{
    public required string Namespace { get; set; }
    public bool IsReusable => false;

    public virtual IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(Namespace, nameof(Namespace));

        if (serviceProvider.TryGetActionCaches(Namespace, out var caches))
        {
            return new ActionCacheRehydrationFilter(
                Namespace,
                new ActionCacheAggregate(caches),
                serviceProvider.GetRequiredService<IActionCacheRehydrator>());
        }
        else
        {
            return default!;
        }
    }
}