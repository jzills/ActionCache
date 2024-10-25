using ActionCache.Common.Enums;
using ActionCache.Common.Filters;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace ActionCache.Filters;

/// <summary>
/// A filter factory attribute that refreshes cached action data based on the specified namespace.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class ActionCacheRefreshFilterFactory : ActionCacheFilterFactoryBase
{
    /// <summary>
    /// Creates an instance of the action cache rehydration filter using the specified service provider.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to access services.</param>
    /// <returns>Returns the constructed filter or null if caches could not be retrieved.</returns>
    public override IFilterMetadata CreateInstance(IServiceProvider serviceProvider) =>
        serviceProvider.GetRequiredService<IActionCacheFilterAbstractFactory>()
            .CreateInstance(Namespace, FilterType.Refresh);
}