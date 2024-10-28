using ActionCache.Common.Caching;
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
    /// <exception cref="InvalidCacheInstanceException"></exception> 
    /// <exception cref="FilterTypeNotSupportedException"></exception>
    public IFilterMetadata CreateInstance(string @namespace, FilterType type) => 
        CreateInstance(@namespace, absoluteExpiration: null, slidingExpiration: null, type);

    /// <inheritdoc/>
    /// <exception cref="InvalidCacheInstanceException"></exception> 
    /// <exception cref="FilterTypeNotSupportedException"></exception>
    public IFilterMetadata CreateInstance(string @namespace, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration, FilterType type)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(@namespace, nameof(@namespace));

        var caches = GetCacheInstances(@namespace, absoluteExpiration, slidingExpiration);
        return CreateHandler(caches, type);
    }

    internal IFilterMetadata CreateHandler(IReadOnlyList<IActionCache> caches, FilterType type)
    {
        if (caches.Count == 0)
        {
            throw new InvalidCacheInstanceException($"No cache instances were able to be created for type \"{type}\".");
        } 
        else
        {
            var cacheHandler = new ActionCacheHandler(caches.First());
            foreach (var cache in caches.Skip(1))
            {
                cacheHandler.SetNext(cache);
            } 

            return CreateFilter(cacheHandler, type);
        }
    }

    internal IFilterMetadata CreateFilter(ActionCacheHandler cache, FilterType type) => 
        type switch
        {
            FilterType.Add      => new ActionCacheFilter(cache, BinderFactory),
            FilterType.Evict    => new ActionCacheEvictionFilter(cache, BinderFactory),
            FilterType.Refresh  => new ActionCacheRefreshFilter(cache, BinderFactory),   
            _                   => throw new FilterTypeNotSupportedException(type)         
        };

    internal IReadOnlyList<IActionCache> GetCacheInstances(string @namespace,
        TimeSpan? absoluteExpiration = null,
        TimeSpan? slidingExpiration = null
    )
    {
        List<IActionCache> cacheInstances = [];

        if (@namespace.Contains(","))
        {
            foreach (var value in @namespace.SplitNamespace())
            {
                AddCacheInstances(value, cacheInstances, absoluteExpiration, slidingExpiration);
            }
        }
        else
        {
            AddCacheInstances(@namespace, cacheInstances, absoluteExpiration, slidingExpiration);
        }

        return cacheInstances.AsReadOnly();
    }

    internal void AddCacheInstances(string @namespace, 
        in List<IActionCache> cacheInstances,
        TimeSpan? absoluteExpiration = null,
        TimeSpan? slidingExpiration = null
    )
    {
        var instances = CreateCacheInstances(@namespace, absoluteExpiration, slidingExpiration);
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