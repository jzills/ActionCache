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

        var cacheFactory = serviceProvider.GetRequiredService<IActionCacheFactory>();

        List<IActionCache> caches = new List<IActionCache>();
        if (Namespaces.Contains(","))
        {
            var @namespaces = Namespaces.Split(",").Select(@namespace => @namespace.Trim());
            var @namespaceCaches = @namespaces.Select(@namespace => cacheFactory.Create(@namespace));
            caches.AddRange(@namespaceCaches!);
        }
        else
        {
            caches.Add(cacheFactory.Create(Namespaces)!);
        }

        return new ActionCacheEvictionFilter(caches.ToArray());
    }
}