using System.Reflection;

namespace ActionCache.Common.Extensions;

public static class ParameterInfoExtensions
{
    public static T? GetCustomAttribute<T>(
        this ParameterInfo parameterInfo
    ) where T : Attribute => parameterInfo
        .GetCustomAttributes(false)
        .OfType<T>()
        .FirstOrDefault();
}