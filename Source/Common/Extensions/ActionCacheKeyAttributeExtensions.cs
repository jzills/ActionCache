using ActionCache.Attributes;

namespace ActionCache.Common.Extensions;

public static class ActionCacheKeyAttributeExtensions
{
    public static IEnumerable<object> GetArguments(
        this IDictionary<string, ActionCacheKeyAttribute> source,
        IDictionary<string, object> args
    ) => source
            .OrderBy(attribute => attribute.Value.Order)
            .Select(attribute => args[attribute.Key]);
}