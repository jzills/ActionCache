using ActionCache.Utilities;

namespace ActionCache.Common.Caching;

public class ActionCacheBaseOptions
{
    public Namespace Namespace { get; set; }
    public ActionCacheEntryOptions EntryOptions { get; set; }
    public ActionCacheRefreshProvider RefreshProvider { get; set; }
}