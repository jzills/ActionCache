namespace ActionCache;

public interface IActionCacheFactory
{
    CacheProvider Provider { get; }
    IActionCache? Create(string @namespace);
}