using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using ActionCache.Enums;
using ActionCache.Filters;

namespace ActionCache.Filters;

public class ActionCacheEvictionFilterFactory : Attribute, IFilterFactory
{
    public required string Namespace { get; set; }
    public bool IsReusable => false;
    
     public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        var cacheProvider = serviceProvider.GetRequiredService<IActionCacheProvider>();
        var cache = cacheProvider.Create(Namespace, ActionCacheTypes.InMemory);
        return new ActionCacheEvictionFilter(cache);
    }
}