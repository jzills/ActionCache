using ActionCache.Common.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ActionCache.Filters;

public class ActionCacheEvictionFilterFactory : Attribute, IFilterFactory
{
    public required string Namespaces { get; set; }
    public bool IsReusable => false;
    
    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(Namespaces, nameof(Namespaces));

        if (serviceProvider.TryGetActionCaches(Namespaces, out var caches))
        {
            return new ActionCacheEvictionFilter(
                new ActionCacheAggregate(caches));
        }
        else
        {
            return default!;
        }
    }
}