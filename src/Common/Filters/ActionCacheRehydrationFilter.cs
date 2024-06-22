using Microsoft.AspNetCore.Mvc.Filters;
using ActionCache.Common;

namespace ActionCache.Filters;

internal class ActionCacheRehydrationFilter : IAsyncResultFilter
{
    protected readonly string Namespace;
    protected readonly IActionCache Cache;
    protected readonly IActionCacheRehydrator Rehydrator;

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
