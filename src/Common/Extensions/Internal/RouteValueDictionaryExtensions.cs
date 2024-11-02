using Microsoft.AspNetCore.Routing;

namespace ActionCache.Common.Extensions.Internal;

internal static class RouteValueDictionaryExtensions
{
    internal static bool TryGetStringValue(this RouteValueDictionary source, string key, out string? value)
    {
        if (source.ContainsKey(key))
        {
            value = (string?)source[key];
            return value is not null;
        }
        else
        {
            value = null;
            return false;
        }
    }
}