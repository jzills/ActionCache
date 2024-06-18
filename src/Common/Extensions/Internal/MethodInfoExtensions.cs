using System.Reflection;
using ActionCache.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace ActionCache.Common.Extensions.Internal;

public static class MethodInfoExtensions
{
    public static bool HasActionCacheAttribute(this MethodInfo source, string @namespace) =>
        source.GetCustomAttribute<ActionCacheAttribute>()?
            .Namespace.Contains(@namespace) ?? false;

    public static bool TryGetRehydrationResult(
        this MethodInfo? methodInfo, 
        object? controller, 
        object?[]? parameters, 
        out RehydrationResult value
    )
    {
        var result = methodInfo?.Invoke(controller, parameters);
        if (result is OkObjectResult okObjectResult)
        {
            value = new RehydrationResult
            {
                Key = string.Join(":", parameters ?? []),
                Value = okObjectResult.Value
            };

            return true;
        }
        else
        {
            value = default!;
            return false;
        }
    }
}