using Microsoft.AspNetCore.Mvc;

namespace ActionCache.Common.Extensions.Internal;

/// <summary>
/// Provides extension methods for <see cref="IActionResult"/> to evaluate response success based on HTTP status codes.
/// </summary>
internal static class IActionResultExtensions
{
    /// <summary>
    /// Determines whether an <see cref="IActionResult"/> represents a successful result, 
    /// based on it being a successful <see cref="ObjectResult"/> or <see cref="StatusCodeResult"/>.
    /// </summary>
    /// <param name="result">The <see cref="IActionResult"/> to evaluate.</param>
    /// <returns><c>true</c> if the result is successful; otherwise, <c>false</c>.</returns>
    internal static bool IsSuccessfulResult(this IActionResult result) =>
        result.IsSuccessfulObjectResult() || 
        result.IsSuccessfulStatusCodeResult();

    /// <summary>
    /// Determines whether an <see cref="IActionResult"/> is a successful <see cref="ObjectResult"/>, 
    /// with a status code in the successful range (200-299).
    /// </summary>
    /// <param name="result">The <see cref="IActionResult"/> to evaluate.</param>
    /// <returns><c>true</c> if the result is a successful <see cref="ObjectResult"/>; otherwise, <c>false</c>.</returns>
    internal static bool IsSuccessfulObjectResult(this IActionResult result) =>
        result is ObjectResult objectResult && 
            objectResult.IsSuccessStatusCode();

    /// <summary>
    /// Determines whether an <see cref="IActionResult"/> is a successful <see cref="StatusCodeResult"/>, 
    /// with a status code in the successful range (200-299).
    /// </summary>
    /// <param name="result">The <see cref="IActionResult"/> to evaluate.</param>
    /// <returns><c>true</c> if the result is a successful <see cref="StatusCodeResult"/>; otherwise, <c>false</c>.</returns>
    internal static bool IsSuccessfulStatusCodeResult(this IActionResult result) =>
        result is StatusCodeResult statusCodeResult &&
            statusCodeResult.IsSuccessStatusCode();
}