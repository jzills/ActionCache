namespace ActionCache;

public interface IActionCacheFactory
{
    IActionCache? Create(string @namespace);
}