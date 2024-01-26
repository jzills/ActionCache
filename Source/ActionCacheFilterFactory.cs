using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using ActionCache.Filters;
using ActionCache.Enums;

namespace ActionCache;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ActionCacheFilterFactory : Attribute, IFilterFactory
{
    public required string Namespace { get; set; }
    public bool IsReusable => false;

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        var cacheProvider = serviceProvider.GetRequiredService<IActionCacheProvider>();
        var cache = cacheProvider.Create(Namespace, ActionCacheTypes.InMemory);
        return new ActionCacheFilter(cache);
    }
}