using ActionCache.Common.Enums;
using ActionCache.Common.Extensions;
using ActionCache.Common.Extensions.Internal;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ActionCache.Filters;

/// <summary>
/// Provides a filter for refreshing action caches.
/// </summary>
internal class ActionCacheRefreshFilter : IAsyncResultFilter
{
    /// <summary>
    /// The action cache manager.
    /// </summary>
    protected readonly IActionCache Cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionCacheRefreshFilter"/> class.
    /// </summary>
    /// <param name="cache">The action cache.</param>
    public ActionCacheRefreshFilter(IActionCache cache) => Cache = cache;
    
    /// <summary>
    /// Asynchronously executes the result operation with cache refresh.
    /// </summary>
    /// <param name="context">The context for result executing.</param>
    /// <param name="next">Delegate for the result execution.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task OnResultExecutionAsync(
        ResultExecutingContext context, 
        ResultExecutionDelegate next
    )
    {
        context.AddCacheStatus(CacheStatus.REFRESH);
        await Cache.RefreshAsync();
        await next();
    }
}