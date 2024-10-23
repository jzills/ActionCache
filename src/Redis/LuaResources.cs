namespace ActionCache.Redis;

internal static class LuaResources
{
    internal const string Remove = $"{nameof(Remove)}.lua";
    internal const string RemoveNamespace = $"{nameof(RemoveNamespace)}.lua";
    internal const string SetHash = $"{nameof(SetHash)}.lua";
}