using ActionCache.Common;
using ActionCache.Common.Enums;
using ActionCache.Common.Filters;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace ActionCache.Filters;

/// <summary>
/// Provides a custom filter factory for caching action results based on the configuration.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class ActionCacheFilterFactory : ActionCacheFilterFactoryBase
{
    /// <summary>
    /// The absolute expiration in milliseconds for a cache entry.
    /// </summary>
    /// <value>Defaults to 0 which represents no expiration.</value>
    public long AbsoluteExpiration { get; set; } = ActionCacheEntryOptions.NoExpiration;

    /// <summary>
    /// The sliding expiration in milliseconds for a cache entry.
    /// </summary>
    /// <value>Defaults to 0 which represents no expiration.</value>
    public long SlidingExpiration { get; set; } = ActionCacheEntryOptions.NoExpiration;

    /// <summary>
    /// Creates an instance of the action cache filter using the specified service provider.
    /// </summary>
    /// <param name="serviceProvider">The service provider to resolve dependencies.</param>
    /// <returns>An instance of an action cache filter.</returns>
    public override IFilterMetadata CreateInstance(IServiceProvider serviceProvider) =>
        serviceProvider.GetRequiredService<IActionCacheFilterAbstractFactory>()
            .CreateInstance(Namespace, FilterType.Add);
}