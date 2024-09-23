using ActionCache.Attributes;
using Newtonsoft.Json;

namespace ActionCache.Common.Extensions;

/// <summary>
/// Provides extension methods for handling cache keys with ActionCacheKey attributes.
/// </summary>
internal static class ActionCacheKeyAttributeExtensions
{
    /// <summary>
    /// Extracts and serializes arguments from a dictionary based on the order specified in ActionCacheKeyAttribute.
    /// </summary>
    /// <param name="source">Dictionary of property names and ActionCacheKeyAttribute.</param>
    /// <param name="args">Dictionary of arguments where the key is the property name.</param>
    /// <returns>An ordered list of arguments, serialized if the corresponding type is a class.</returns>
    internal static IEnumerable<object> GetArguments(
        this IReadOnlyDictionary<string, ActionCacheKeyAttribute> source,
        IDictionary<string, object> actionArguments
    )
    {
        var mappedArguments = new List<object>(source.Count);
        foreach (var attribute in source.OrderBy(attribute => attribute.Value.Order))
        {
            var argument = actionArguments[attribute.Key];
            if (ShouldSerialize(argument.GetType()))
            {
                mappedArguments.Add(JsonConvert.SerializeObject(argument));
            }
            else
            {
                mappedArguments.Add(argument);
            }
        }

        return mappedArguments;
    }

    private static bool ShouldSerialize(Type type) => type != typeof(string) && type.IsClass;
}