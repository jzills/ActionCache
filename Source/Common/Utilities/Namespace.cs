namespace ActionCache.Utilities;

public record struct Namespace(string @namespace)
{
    public string Create(string key) => $"{@namespace}:{key}";

}