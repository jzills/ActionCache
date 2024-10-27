using ActionCache.Utilities;

namespace ActionCache.Common.Caching;

/// <summary>
/// A class responsible for executing controller actions for cache keys
/// </summary>
public interface IActionCacheRefreshProvider
{
    /// <summary>
    /// Invokes the method representing a controller action on the specified namespace and keys. 
    /// </summary>
    /// <param name="namespace">The namespace to refresh against.</param>
    /// <param name="keys">The keys to be checked against.</param>
    /// <returns>A readonly dictionary of results where the key is the cache entry key and the value is the result of executing the controller action stored for that key.</returns>
    IReadOnlyDictionary<string, object?> GetRefreshResults(Namespace @namespace, IEnumerable<string> keys);
}