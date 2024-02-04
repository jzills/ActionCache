using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ActionCache.Common.Extensions;

public static class ActionExecutedContextExtensions
{
    public static bool TryGetObjectResultValue(this ActionExecutedContext context, out object result)
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