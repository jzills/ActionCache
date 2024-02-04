using ActionCache.Attributes;

namespace ActionCache.Common.Extensions;

internal static class ActionCacheKeyAttributeExtensions
{
    internal static IEnumerable<object> GetArguments(
        this IDictionary<string, ActionCacheKeyAttribute> source,
        IDictionary<string, object> args
    ) => source
            .OrderBy(attribute => attribute.Value.Order)
            .Select(attribute => args[attribute.Key]);
}