using ActionCache.Common.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace ActionCache.Filters;

public class ActionCacheEvictionFilterFactory : Attribute, IFilterFactory
{
    public required string Namespaces { get; set; }
    public bool IsReusable => false;
    
    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(Namespaces, nameof(Namespaces));

        if (serviceProvider.TryGetActionCaches(Namespaces, out var caches))
        {
            return new ActionCacheEvictionFilter(
                new ActionCacheAggregate(caches));
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