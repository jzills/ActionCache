using Microsoft.Extensions.DependencyInjection;

namespace ActionCache.Common.Extensions;

internal static class IServiceProviderExtensions
{
    internal static IEnumerable<IActionCache?>? GetActionCaches(
        this IServiceProvider serviceProvider, 
        string @namespace
    ) => serviceProvider
            .GetActionCacheFactories()
            .Select(factory => factory.Create(@namespace));

    internal static bool TryGetActionCaches(
        this IServiceProvider serviceProvider, 
        string namespaces,
        out IEnumerable<IActionCache> caches
    )
    {
        if (namespaces.Contains(","))
        {
            caches = GetNamespaces(namespaces)
                .SelectMany(@namespace => 
                    serviceProvider.GetActionCaches(@namespace)!)!;

            return EnsureAllCachesExist(caches);
        }
        else
        {
            caches = serviceProvider.GetActionCaches(@namespaces)!;
            return EnsureAllCachesExist(caches);
        }
    }

    internal static bool TryGetActionCacheFactory(
        this IServiceProvider serviceProvider,
        CacheProvider cacheProvider,
        out IActionCacheFactory cacheFactory
    )
    {
        cacheFactory = serviceProvider
            .GetActionCacheFactories()
            .FirstOrDefault(cacheFactory => 
                cacheFactory.Provider == cacheProvider)!;

        return cacheFactory is not null;
    }

    internal static IEnumerable<IActionCacheFactory> GetActionCacheFactories(
        this IServiceProvider serviceProvider
    ) => serviceProvider.GetRequiredService<IEnumerable<IActionCacheFactory>>();

    private static IEnumerable<string> GetNamespaces(string namespaces) =>
        namespaces.Split(",").Select(@namespace => 
            @namespace.Trim());

    private static bool EnsureAllCachesExist(IEnumerable<IActionCache?>? caches) =>
        (caches?.Any() ?? false) && 
         caches.All(cache => cache is not null);
}