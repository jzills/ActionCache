using ActionCache.Enums;

namespace ActionCache;

public interface IActionCacheProvider
{
    IActionCache Create(string @namespace, ActionCacheTypes type);
}