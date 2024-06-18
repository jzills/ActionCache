using System.Reflection;
using System.Text.Json;
using ActionCache.Attributes;

namespace ActionCache.Common.Extensions;

internal static class ParameterInfoExtensions
{
    internal static T? GetCustomAttribute<T>(
        this ParameterInfo parameterInfo
    ) where T : Attribute => parameterInfo
        .GetCustomAttributes(false)
        .OfType<T>()
        .FirstOrDefault();

    internal static bool TryGetValue(
        this ParameterInfo parameterInfo, 
        IDictionary<string, JsonElement>? actionArg,
        out (int Order, object? Value) actionArgConversion
    )
    {
        if (actionArg is null)
        {
            actionArgConversion = default;
            return false;
        }

        var attribute = parameterInfo.GetCustomAttribute<ActionCacheKeyAttribute>();
        if (attribute is null)
        {
            actionArgConversion = default;
            return false;
        }

        var actionValue = actionArg.First(arg => arg.Key == parameterInfo.Name);
        var actionValueConversion = actionValue.Value.Deserialize(parameterInfo.ParameterType);
        actionArgConversion = (attribute.Order, actionValueConversion);
        return true;
    }
}