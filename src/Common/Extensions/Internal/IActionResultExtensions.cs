using Microsoft.AspNetCore.Mvc;

namespace ActionCache.Common.Extensions.Internal;

internal static class IActionResultExtensions
{
    internal static bool IsSuccessfulResult(this IActionResult result) =>
        result.IsSuccessfulObjectResult() || 
        result.IsSuccessfulStatusCodeResult();

    internal static bool IsSuccessfulObjectResult(this IActionResult result) =>
        result is ObjectResult objectResult && 
            objectResult.IsSuccessStatusCode();

    internal static bool IsSuccessfulStatusCodeResult(this IActionResult result) =>
        result is StatusCodeResult statusCodeResult &&
            statusCodeResult.IsSuccessStatusCode();
}