using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace ActionCache.Filters;

public class ActionCacheEvictionFilterFactory : Attribute, IFilterFactory
{
    public required string Namespace { get; set; }
    public bool IsReusable => false;
    
    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(Namespace, nameof(Namespace));

        var cacheFactory = serviceProvider.GetRequiredService<IActionCacheFactory>();
        var cache = cacheFactory.Create(Namespace);

        ArgumentNullException.ThrowIfNull(cache, nameof(cache));

        return new ActionCacheEvictionFilter(cache);
    }
}