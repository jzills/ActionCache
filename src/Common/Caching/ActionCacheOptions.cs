namespace ActionCache.Common;

public class ActionCacheOptions
{
    public readonly ActionCacheEntryOptions EntryOptions = new();
    public readonly IDictionary<CacheType, bool> EnabledCaches = 
        new Dictionary<CacheType, bool>
        {
            [CacheType.Redis]     = false,
            [CacheType.Memory]    = false,
            [CacheType.SqlServer] = false
        };
}