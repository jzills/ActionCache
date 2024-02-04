using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ActionCache.Common.Extensions;

internal static class ActionExecutedContextExtensions
{
    internal static bool TryGetObjectResultValue(this ActionExecutedContext context, out object result)
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