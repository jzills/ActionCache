using Microsoft.AspNetCore.Mvc.Filters;
using ActionCache.Common;

namespace ActionCache.Filters;

public class ActionCacheRehydrationFilter : IAsyncResultFilter
{
    protected readonly IActionCache Cache;
    protected readonly IActionCacheRehydrator Rehydrator;

    public ActionCacheRehydrationFilter(
        IActionCache cache,
        IActionCacheRehydrator rehydrator
    )
    {
        Cache = cache;
        Rehydrator = rehydrator;
    }
    
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var rehydrationResults = await Rehydrator.GetRehydrationResultsAsync("Namespace1");
        if (rehydrationResults.Any())
        {
            await Task.WhenAll(rehydrationResults
                .Select(result => Cache.SetAsync(result.Key, result.Value)));
        }

        await next();
    }
}
