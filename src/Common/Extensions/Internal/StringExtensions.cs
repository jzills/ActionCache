namespace ActionCache.Common.Extensions.Internal;

internal static class StringExtensions
{
    public static IEnumerable<string> SplitNamespace(this string values) =>
        values.Split(',', 
            StringSplitOptions.TrimEntries | 
            StringSplitOptions.RemoveEmptyEntries
        );
}