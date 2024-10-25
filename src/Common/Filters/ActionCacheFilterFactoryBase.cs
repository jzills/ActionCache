using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ActionCache.Filters;

/// <summary>
/// Provides a base factory for creating instances of action cache filters.
/// </summary>
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

    /// <inheritdoc/>
    public abstract IFilterMetadata CreateInstance(IServiceProvider serviceProvider);
}