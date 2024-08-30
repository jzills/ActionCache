using ActionCache.Common;
using ActionCache.Common.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace ActionCache.Filters;

/// <summary>
/// A filter factory attribute that rehydrates cached action data based on the specified namespace.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
internal class ActionCacheRehydrationFilterFactory : Attribute, IFilterFactory
{
    /// <summary>
    /// Gets or sets the namespace used to retrieve and group the action caches.
    /// </summary>
    public required string Namespace { get; set; }

    /// <summary>
    /// Determines whether multiple instances of the filter can be reused. Returns false indicating non-reusability.
    /// </summary>
    public bool IsReusable => false;

    /// <summary>
    /// Creates an instance of the action cache rehydration filter using the specified service provider.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to access services.</param>
    /// <returns>Returns the constructed filter or null if caches could not be retrieved.</returns>
    public virtual IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(Namespace, nameof(Namespace));

        if (serviceProvider.TryGetActionCaches(Namespace, out var caches))
        {
            return new ActionCacheRehydrationFilter(
                Namespace,
                new ActionCacheAggregate(caches),
                serviceProvider.GetRequiredService<IActionCacheRehydrator>());
        }
        else
        {
            return default!;
        }
    }
}