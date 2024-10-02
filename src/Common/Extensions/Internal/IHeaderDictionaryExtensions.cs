using ActionCache.Common.Enums;
using Microsoft.AspNetCore.Http;

namespace ActionCache.Common.Extensions.Internal;

internal static class IHeaderDictionaryExtensions
{
    internal static void AddCacheStatus(
        this IHeaderDictionary headers, 
        CacheStatus status
    ) => headers.Add("Cache-Status", Enum.GetName(status));
}