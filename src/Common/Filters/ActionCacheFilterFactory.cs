using ActionCache.Common.Extensions;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.DependencyInjection;

namespace ActionCache.Filters;

/// <summary>
/// Provides a custom filter factory for caching action results based on the configuration.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ActionCacheFilterFactory : Attribute, IFilterFactory
{
    /// <summary>
    /// Gets or sets the namespace used to identify the related action caches.
    /// </summary>
    [StringSyntax("Route")] 
    public required string Namespace { get; set; }

    public TimeSpan? AbsoluteExpiration { get; set; }

    public TimeSpan? SlidingExpiration { get; set; }

    /// <summary>
    /// Indicates whether multiple instances of the filter attribute are reusable.
    /// </summary>
    public bool IsReusable => false;

    /// <summary>
    /// Creates an instance of the action cache filter using the specified service provider.
    /// </summary>
    /// <param name="serviceProvider">The service provider to resolve dependencies.</param>
    /// <returns>An instance of an action cache filter.</returns>
    public virtual IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(Namespace, nameof(Namespace));

        if (serviceProvider.TryGetActionCaches(Namespace, out var caches))
        {
            var binderFactory = serviceProvider.GetRequiredService<TemplateBinderFactory>();
            return new ActionCacheFilter(caches.First(), binderFactory);
        }
        else
        {
            return default!;
        }
    }
}