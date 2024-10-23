namespace ActionCache.Redis;

internal static class LuaResourceEnum
{
    internal static readonly string SetJsonWithKeySet = Format(nameof(SetJsonWithKeySet));
    internal static readonly string SetHash = Format(nameof(SetHash));
    internal static readonly string UnlinkNamespace = Format(nameof(UnlinkNamespace));
    internal static readonly string Remove = Format(nameof(Remove));
    internal static readonly string RemoveNamespace = Format(nameof(RemoveNamespace));
    private static string Format(string resource) => $"{resource}.lua";
}