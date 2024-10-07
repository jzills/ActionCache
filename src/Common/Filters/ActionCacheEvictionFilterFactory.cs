using System.Diagnostics.CodeAnalysis;
using ActionCache.Common.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ActionCache.Filters;

/// <summary>
/// Provides a factory for creating instances of <see cref="ActionCacheEvictionFilter"/> based on specified namespaces.
/// </summary>
public class ActionCacheEvictionFilterFactory : Attribute, IFilterFactory
{
    /// <summary>
    /// Gets or sets the namespaces used to identify the related action caches.
    /// </summary>
    [StringSyntax("Route")] 
    public required string Namespace { get; set; }

    /// <summary>
    /// Determines whether multiple instances of the filter are reusable. Returns false indicating non-reusability.
    /// </summary>
    public bool IsReusable => false;
    
    /// <summary>
    /// Creates an instance of an action cache eviction filter using the provided service provider.
    /// </summary>
    /// <param name="serviceProvider">The service provider to use for getting action cache services.</param>
    /// <returns>An instance of <see cref="IFilterMetadata"/> representing the action cache eviction filter.</returns>
    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(Namespace, nameof(Namespace));

        if (serviceProvider.TryGetActionCaches(Namespace, out var caches))
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