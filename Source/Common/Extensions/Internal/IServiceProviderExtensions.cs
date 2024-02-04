using Microsoft.Extensions.DependencyInjection;

namespace ActionCache.Common.Extensions;

internal static class IServiceProviderExtensions
{
    internal static IEnumerable<IActionCache?>? GetActionCaches(
        this IServiceProvider serviceProvider, 
        string @namespace
    ) => serviceProvider
            .GetRequiredService<IEnumerable<IActionCacheFactory>>()
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

    private static IEnumerable<string> GetNamespaces(string namespaces) =>
        namespaces.Split(",").Select(@namespace => @namespace.Trim());

    private static bool EnsureAllCachesExist(IEnumerable<IActionCache?>? caches) =>
        (caches?.Any() ?? false) && 
         caches.All(cache => cache is not null);
}