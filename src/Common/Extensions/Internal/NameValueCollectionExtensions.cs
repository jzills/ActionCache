using System.Collections.Specialized;
using System.Text.Json;

namespace ActionCache.Common.Extensions.Internal;

internal static class NameValueCollectionExtensions
{
    internal static T? ParseValueAsJson<T>(this NameValueCollection nameValues, string name) where T : class
    {
        var json = nameValues.Get(name);
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<T>(json);
        }
        catch (Exception)
        {
            return null;
        }
    }
}