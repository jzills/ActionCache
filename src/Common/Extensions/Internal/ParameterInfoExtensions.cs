using ActionCache.Attributes;
using System.Reflection;
using System.Text.Json;

namespace ActionCache.Common.Extensions;

internal static class ParameterInfoExtensions
{
    /// <summary>
    /// Gets a custom attribute of the specified type from the parameter.
    /// </summary>
    /// <typeparam name="T">The type of attribute to search for.</typeparam>
    /// <param name="parameterInfo">The parameter info to search for attributes.</param>
    /// <returns>The first custom attribute of type T found, or null if none are found.</returns>
    internal static T? GetCustomAttribute<T>(
        this ParameterInfo parameterInfo
    ) where T : Attribute => parameterInfo
        .GetCustomAttributes(false)
        .OfType<T>()
        .FirstOrDefault();

    /// <summary>
    /// Tries to get a value from the action argument dictionary and converts it to its associated type.
    /// </summary>
    /// <param name="parameterInfo">The parameter to extract information from.</param>
    /// <param name="actionArg">The dictionary containing action arguments.</param>
    /// <param name="actionArgConversion">Outputs the order and converted value of the action argument.</param>
    /// <returns>True if the conversion is successful; otherwise false.</returns>
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