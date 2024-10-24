using ActionCache.Common.Extensions.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace ActionCache.Common.Extensions;

/// <summary>
/// Extension methods for IServiceProvider to handle action caches.
/// </summary>
internal static class IServiceProviderExtensions
{
    /// <summary>
    /// Retrieves a collection of IActionCache instances for the specified namespace.
    /// </summary>
    /// <param name="serviceProvider">The service provider instance.</param>
    /// <param name="namespace">The namespace to retrieve caches for.</param>
    /// <returns>A collection of IActionCache instances or null.</returns>
    internal static IEnumerable<IActionCache?>? GetActionCaches(
        this IServiceProvider serviceProvider, 
        string @namespace,
        TimeSpan? absoluteExpiration = null,
        TimeSpan? slidingExpiration = null
    ) => serviceProvider
            .GetActionCacheFactories()
            .Select(factory => factory.Create(@namespace, absoluteExpiration, slidingExpiration));

    /// <summary>
    /// Tries to retrieve action caches for specified namespaces and provides them out if successful.
    /// </summary>
    /// <param name="serviceProvider">The service provider instance.</param>
    /// <param name="namespaces">A comma-separated list of namespaces.</param>
    /// <param name="caches">Out parameter that returns the retrieved caches.</param>
    /// <returns>True if all specified caches exist, otherwise false.</returns>
    internal static bool TryGetActionCaches(
        this IServiceProvider serviceProvider, 
        string @namespace,
        TimeSpan? absoluteExpiration,
        TimeSpan? slidingExpiration,
        out IEnumerable<IActionCache> caches
    )
    {
        if (@namespace.Contains(","))
        {
            caches = GetNamespaces(@namespace)
                .SelectMany(@namespace => 
                    serviceProvider.GetActionCaches(@namespace, absoluteExpiration, slidingExpiration)!)!;

            return EnsureAllCachesExist(caches);
        }
        else
        {
            caches = serviceProvider.GetActionCaches(@namespace, absoluteExpiration, slidingExpiration)!;
            return EnsureAllCachesExist(caches);
        }
    }

    /// <summary>
    /// Tries to retrieve action caches for specified namespaces and provides them out if successful.
    /// </summary>
    /// <param name="serviceProvider">The service provider instance.</param>
    /// <param name="namespaces">A comma-separated list of namespaces.</param>
    /// <param name="caches">Out parameter that returns the retrieved caches.</param>
    /// <returns>True if all specified caches exist, otherwise false.</returns>
    internal static bool TryGetActionCaches(
        this IServiceProvider serviceProvider, 
        string @namespace,
        out IEnumerable<IActionCache> caches
    )
    {
        if (@namespace.Contains(","))
        {
            caches = GetNamespaces(@namespace)
                .SelectMany(@namespace => 
                    serviceProvider.GetActionCaches(@namespace)!)!;

            return EnsureAllCachesExist(caches);
        }
        else
        {
            caches = serviceProvider.GetActionCaches(@namespace)!;
            return EnsureAllCachesExist(caches);
        }
    }

    /// <summary>
    /// Tries to get an action cache factory for the specified cache type.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="CacheType">The type of cache.</param>
    /// <param name="cacheFactory">Out parameter that returns the cache factory if found.</param>
    /// <returns>True if a factory is found, otherwise false.</returns>
    internal static bool TryGetActionCacheFactory(
        this IServiceProvider serviceProvider,
        CacheType CacheType,
        out IActionCacheFactory cacheFactory
    )
    {
        cacheFactory = serviceProvider
            .GetActionCacheFactories()
            .FirstOrDefault(cacheFactory => 
                cacheFactory.Type == CacheType)!;

        return cacheFactory is not null;
    }

    /// <summary>
    /// Gets all action cache factories available in the service provider.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>A collection of action cache factories.</returns>
    internal static IEnumerable<IActionCacheFactory> GetActionCacheFactories(
        this IServiceProvider serviceProvider
    ) => serviceProvider.GetRequiredService<IEnumerable<IActionCacheFactory>>();

    /// <summary>
    /// Splits the provided comma-separated namespace strings into an enumerable of trimmed strings.
    /// </summary>
    /// <param name="namespaces">The comma-separated namespaces string.</param>
    /// <returns>IEnumerable of namespace strings.</returns>
    private static IEnumerable<string> GetNamespaces(string namespaces) =>
        namespaces.Split(",").Select(@namespace => 
            @namespace.Trim());

    /// <summary>
    /// Ensures that all caches in the provided collection exist and are not null.
    /// </summary>
    /// <param name="caches">The collection of caches.</param>
    /// <returns>True if all caches exist, otherwise false.</returns>
    private static bool EnsureAllCachesExist(IEnumerable<IActionCache?>? caches) =>
        caches.Some() && 
        caches.All(cache => cache is not null);
}