namespace ActionCache.Common.Extensions.Internal;

public static class IEnumerableExtensions
{
    public static bool Some<T>(this IEnumerable<T>? source) 
        => source is not null && source.Any();
}