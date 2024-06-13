using System.Reflection;
using System.Text.Json;
using ActionCache.Attributes;

namespace ActionCache.Redis.Extensions;

public static class ParameterInfoExtensions
{
    public static bool TryGetValue(
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