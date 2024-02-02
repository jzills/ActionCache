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
        string @namespace,
        out IEnumerable<IActionCache> caches
    )
    {
        caches = serviceProvider.GetActionCaches(@namespace)!;
        return caches?.Any(cache => cache is null) ?? false;
    }
}