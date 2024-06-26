namespace ActionCache;

public interface IActionCacheFactory
{
    CacheType Type { get; }
    IActionCache? Create(string @namespace);
}