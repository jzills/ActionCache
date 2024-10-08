using ActionCache.Common.Serialization;
using System.Collections.Specialized;

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
            return CacheJsonSerializer.Deserialize<T>(json);
        }
        catch (Exception)
        {
            return null;
        }
    }
}