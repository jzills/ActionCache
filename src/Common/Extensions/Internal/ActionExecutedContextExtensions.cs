using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ActionCache.Common.Extensions;

/// <summary>
/// Provides extension methods for <see cref="ActionExecutedContext"/>.
/// </summary>
internal static class ActionExecutedContextExtensions
{
    /// <summary>
    /// Attempts to retrieve the value from the <see cref="OkObjectResult"/> of an <see cref="ActionExecutedContext"/>.
    /// </summary>
    /// <param name="context">The <see cref="ActionExecutedContext"/> to extract the result from.</param>
    /// <param name="result">The extracted result object if the operation is successful.</param>
    /// <returns>True if the operation was successful and the result is not null; otherwise, false.</returns>
    internal static bool TryGetOkObjectResultValue(this ActionExecutedContext context, out object result)
    {
        if (context is not null &&
            context.Result is OkObjectResult okObjectResult)
        {
            result = okObjectResult.Value;
            return true;
        }
        else
        {
            result = default!;
            return false;
        }
    }
}