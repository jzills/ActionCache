using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace ActionCache.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ActionCacheFilterFactory : Attribute, IFilterFactory
{
    public required string Namespace { get; set; }
    public bool IsReusable => false;

    public virtual IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(Namespace, nameof(Namespace));

        var cacheFactory = serviceProvider.GetRequiredService<IActionCacheFactory>();
        var cache = cacheFactory.Create(Namespace);

        ArgumentNullException.ThrowIfNull(cache, nameof(cache));

        return new ActionCacheFilter(cache);
    }
}