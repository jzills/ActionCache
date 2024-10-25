using ActionCache.Common.Enums;
using ActionCache.Common.Extensions.Internal;
using ActionCache.Exceptions;
using ActionCache.Filters;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing.Template;

namespace ActionCache.Common.Filters;

/// <summary>
/// The abstract factory for creating cache filters.
/// </summary>
public class ActionCacheFilterAbstractFactory : IActionCacheFilterAbstractFactory
{
    /// <summary>
    /// The cache factories used to create caches.
    /// </summary>
    protected readonly IEnumerable<IActionCacheFactory> CacheFactories;

    /// <summary>
    /// The template binder for parsing route parameters for templated namespaces.
    /// </summary>
    protected readonly TemplateBinderFactory BinderFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionCacheEvictionFilter"/> class.
    /// </summary>
    /// <param name="cacheFactories">The cache factories used to create caches.</param>
    /// <param name="binderFactory">The template binder for parsing route parameters for templated namespaces.</param>
    public ActionCacheFilterAbstractFactory(
        IEnumerable<IActionCacheFactory> cacheFactories,
        TemplateBinderFactory binderFactory
    )
    {
        CacheFactories = cacheFactories;
        BinderFactory = binderFactory;
    }   

    /// <inheritdoc/>
    /// <exception cref="FilterTypeNotSupportedException"></exception>
    public IFilterMetadata CreateInstance(string @namespace, FilterType type)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(@namespace, nameof(@namespace));

        // TODO: Allow multiple cache instances
        var caches = GetCacheInstances(@namespace).First(); 
        return type switch
        {
            FilterType.Add      => new ActionCacheFilter(caches, BinderFactory),
            FilterType.Evict    => new ActionCacheEvictionFilter(caches, BinderFactory),
            FilterType.Refresh  => new ActionCacheRefreshFilter(caches, BinderFactory),   
            _                   => throw new FilterTypeNotSupportedException(type)         
        };
    }

    internal IReadOnlyList<IActionCache> GetCacheInstances(string @namespace)
    {
        List<IActionCache> cacheInstances = [];

        if (@namespace.Contains(","))
        {
            foreach (var value in @namespace.SplitNamespace())
            {
                AddCacheInstances(value, cacheInstances);
            }
        }
        else
        {
            AddCacheInstances(@namespace, cacheInstances);
        }

        return cacheInstances.AsReadOnly();
    }

    internal void AddCacheInstances(string @namespace, in List<IActionCache> cacheInstances)
    {
        var instances = CreateCacheInstances(@namespace);
        if (instances is null || instances.Any(instance => instance is null))
        {
            throw new InvalidCacheInstanceException();
        }
        else
        {
            cacheInstances.AddRange(instances!);
        }
    }

    internal IEnumerable<IActionCache?>? CreateCacheInstances( 
        string @namespace,
        TimeSpan? absoluteExpiration = null,
        TimeSpan? slidingExpiration = null
    ) => CacheFactories.Select(factory => factory.Create(@namespace, absoluteExpiration, slidingExpiration));
}