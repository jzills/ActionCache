using System.Reflection;

namespace ActionCache.Common.Extensions;

internal static class ParameterInfoExtensions
{
    internal static T? GetCustomAttribute<T>(
        this ParameterInfo parameterInfo
    ) where T : Attribute => parameterInfo
        .GetCustomAttributes(false)
        .OfType<T>()
        .FirstOrDefault();
}