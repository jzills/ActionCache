using ActionCache.Common.Extensions.Internal;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace ActionCache.Filters;

public abstract class ActionCacheFilterFactoryBase : Attribute, IFilterFactory
{
    /// <summary>
    /// Gets or sets the namespace used to identify the related action caches.
    /// </summary>
    [StringSyntax("Route")] 
    public required string Namespace { get; set; }
    
    /// <summary>
    /// Indicates whether multiple instances of the filter attribute are reusable.
    /// </summary>
    public bool IsReusable => false;

    public abstract IFilterMetadata CreateInstance(IServiceProvider serviceProvider);

    internal IReadOnlyList<IActionCache> GetCacheInstances(IServiceProvider serviceProvider)
    {
        List<IActionCache> cacheInstances = [];

        if (Namespace.Contains(","))
        {
            foreach (var @namespace in Namespace.SplitNamespace())
            {
                AddCacheInstances(serviceProvider, @namespace, cacheInstances);
            }
        }
        else
        {
            AddCacheInstances(serviceProvider, Namespace, cacheInstances);
        }

        return cacheInstances.AsReadOnly();
    }

    internal void AddCacheInstances(IServiceProvider serviceProvider, string @namespace, in List<IActionCache> cacheInstances)
    {
        var instances = CreateCacheInstances(serviceProvider, @namespace);
        if (instances is null || instances.Any(instance => instance is null))
        {
            throw new Exception();
        }
        else
        {
            cacheInstances.AddRange(instances);
        }
    }

    internal IEnumerable<IActionCache?>? CreateCacheInstances(
        IServiceProvider serviceProvider, 
        string @namespace,
        TimeSpan? absoluteExpiration = null,
        TimeSpan? slidingExpiration = null
    ) => GetCacheFactories(serviceProvider)
            .Select(factory => factory.Create(@namespace, absoluteExpiration, slidingExpiration));

    private IEnumerable<IActionCacheFactory> GetCacheFactories(
        IServiceProvider serviceProvider
    ) => serviceProvider.GetRequiredService<IEnumerable<IActionCacheFactory>>();
}