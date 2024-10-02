using ActionCache.Common.Enums;
using ActionCache.Common.Extensions.Internal;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ActionCache.Common.Extensions;

/// <summary>
/// Provides extensions for <see cref="ResultExecutingContextExtensions"/>.
/// </summary>
internal static class ResultExecutingContextExtensions
{
    internal static void AddCacheStatus(
        this ResultExecutingContext context, 
        CacheStatus status
    ) => context.HttpContext.Response.Headers.AddCacheStatus(status);
}