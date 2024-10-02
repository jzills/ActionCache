using ActionCache.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace ActionCache.Common.Extensions.Internal;

/// <summary>
/// Provides extension methods for <see cref="MethodInfo"/>.
/// </summary>
internal static class MethodInfoExtensions
{
    /// <summary>
    /// Determines if the method has an <see cref="ActionCacheAttribute"/> that contains the specified namespace.
    /// </summary>
    /// <param name="source">The source method info.</param>
    /// <param name="namespace">The namespace to check against the attribute's namespace.</param>
    /// <returns>true if an <see cref="ActionCacheAttribute"/> exists and contains the namespace; otherwise, false.</returns>
    public static bool HasActionCacheAttribute(this MethodInfo source, string @namespace) =>
        source.GetCustomAttribute<ActionCacheAttribute>()?
            .Namespace.Contains(@namespace) ?? false;

    /// <summary>
    /// Tries to get the refresh result from the method execution using specified parameters.
    /// </summary>
    /// <param name="methodInfo">The method to invoke.</param>
    /// <param name="controller">The controller instance on which to invoke the method.</param>
    /// <param name="parameters">Parameters to use during method invocation.</param>
    /// <param name="value">The output refresh result if the method execution is successful.</param>
    /// <returns>true if the method returns an <see cref="OkObjectResult"/>; otherwise, false.</returns>
    public static bool TryGetRefreshResult(
        this MethodInfo? methodInfo, 
        object? controller, 
        object?[]? parameters, 
        out RefreshResult value
    )
    {
        var result = methodInfo?.Invoke(controller, parameters);
        if (result is OkObjectResult okObjectResult)
        {
            value = new RefreshResult
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