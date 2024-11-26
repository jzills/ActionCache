using ActionCache.Common.Enums;
using ActionCache.Common.Extensions.Internal;
using ActionCache.Common.Keys;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ActionCache.Common.Extensions;

/// <summary>
/// Provides extensions for <see cref="ActionExecutingContext"/>.
/// </summary>
internal static class ActionExecutingContextExtensions
{
    /// <summary>
    /// Tries to generate a cache key from the given context.
    /// </summary>
    /// <param name="context">The action executing context containing necessary data.</param>
    /// <param name="key">Outputs the generated cache key.</param>
    /// <returns>True if a key is successfully generated, otherwise false.</returns>
    internal static bool TryGetKey(this ActionExecutingContext context, out string key) 
    {
        key = new ActionCacheKeyBuilder()
            .WithRouteValues(context.RouteData.Values)
            .WithActionArguments(context.ActionArguments)
            .Build();

        if (string.IsNullOrWhiteSpace(key))
        {
            key = default!;
            return false;
        }

        return true;
    }

    /// <summary>
    /// Adds the specified cache status to the HTTP response headers of the given action executing context.
    /// </summary>
    /// <param name="context">
    /// The <see cref="ActionExecutingContext"/> containing the HTTP context where the cache status will be added.
    /// </param>
    /// <param name="status">
    /// The <see cref="CacheStatus"/> to include in the response headers.
    /// </param>
    internal static void AddCacheStatus(
        this ActionExecutingContext context, 
        CacheStatus status
    ) => context.HttpContext.Response.Headers.AddCacheStatus(status);
}