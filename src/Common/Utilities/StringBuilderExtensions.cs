using System.Text;

namespace ActionCache.Common.Utilities;

public static class StringBuilderExtensions
{
    public static StringBuilder AppendJoinNonNull(this StringBuilder builder, char separator, params string?[] values)
    {
        var nonNullValues = values.Where(value => !string.IsNullOrWhiteSpace(value));
        return builder.AppendJoin(separator, nonNullValues ?? []);
    }
}