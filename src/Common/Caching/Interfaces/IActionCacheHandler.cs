namespace ActionCache.Common.Caching;

public interface IActionCacheHandler
{
    IActionCache SetNext(IActionCache next);
}