namespace ActionCache.Common;

public class ActionCacheOptions
{
    public readonly ActionCacheEntryOptions EntryOptions = new();
    public readonly IDictionary<CacheProvider, bool> EnabledCaches = 
        new Dictionary<CacheProvider, bool>
        {
            [CacheProvider.Redis]     = false,
            [CacheProvider.Memory]    = false,
            [CacheProvider.SqlServer] = false
        };
}