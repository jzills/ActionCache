using ActionCache.Common.Enums;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ActionCache.Common.Filters;

public interface IActionCacheFilterAbstractFactory
{
    /// <summary>
    /// Creates an instance of a cache filter.
    /// </summary>
    /// <param name="namespace">The namespace for the caches used in the filter.</param>
    /// <param name="type">The filter type to create.</param>
    /// <returns>An implementation of IFilterMetadata.</returns>
    public IFilterMetadata CreateInstance(string @namespace, FilterType type);
}