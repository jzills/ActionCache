using ActionCache.Common;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ActionCache.Filters;

/// <summary>
/// Provides a filter for rehydrating action caches.
/// </summary>
internal class ActionCacheRehydrationFilter : IAsyncResultFilter
{
    /// <summary>
    /// The namespace associated with the cache.
    /// </summary>
    protected readonly string Namespace;

    /// <summary>
    /// The action cache manager.
    /// </summary>
    protected readonly IActionCache Cache;

    /// <summary>
    /// The rehydrator for the action cache.
    /// </summary>
    protected readonly IActionCacheRehydrator Rehydrator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionCacheRehydrationFilter"/> class.
    /// </summary>
    /// <param name="namespace">The namespace of the cache.</param>
    /// <param name="cache">The action cache.</param>
    /// <param name="rehydrator">The rehydrator for the action cache.</param>
    public ActionCacheRehydrationFilter(
        string @namespace,
        IActionCache cache,
        IActionCacheRehydrator rehydrator
    )
    {
        Namespace = @namespace;
        Cache = cache;
        Rehydrator = rehydrator;
    }
    
    /// <summary>
    /// Asynchronously executes the result operation with cache rehydration.
    /// </summary>
    /// <param name="context">The context for result executing.</param>
    /// <param name="next">Delegate for the result execution.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var rehydrationResults = await Rehydrator.GetRehydrationResultsAsync(Namespace);
        if (rehydrationResults.Any())
        {
            await Task.WhenAll(rehydrationResults
                .Select(result => Cache.SetAsync(result.Key, result.Value)));
        }

        await next();
    }
}